using IPTVChannelListProxy.ScheduledJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace IPTVChannelListProxy.Extensions
{
    public static class QuartzExtensions
    {
        public static async void UseQuartz(this IApplicationBuilder app)
        {
            IScheduler scheduler = app.ApplicationServices.GetService<IScheduler>();
            scheduler.JobFactory = app.ApplicationServices.GetService<IJobFactory>();


            await scheduler.CreateScheduleJob<UpdateDataScheduledJob>(trigger => trigger
                .WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
                .StartAt(DateTimeOffset.Now.AddMinutes(5))
            );


            await scheduler.Start();
        }

        public static void AddQuartz(this IServiceCollection services)
        {
            AddQuartz(services, null);
        }

        public static async void AddQuartz(this IServiceCollection services, Action<NameValueCollection> configuration)
        {
            var props = new NameValueCollection();
            configuration?.Invoke(props);

            ISchedulerFactory factory = new StdSchedulerFactory(props);
            IScheduler scheduler = await factory.GetScheduler();

            services.AddSingleton(scheduler);
            services.AddSingleton<IJobFactory, JobFactory>();

            services.AddTransient<UpdateDataScheduledJob>();
        }

        public static async Task CreateScheduleJob<TJob>(this IScheduler scheduler, Func<TriggerBuilder, TriggerBuilder> action) where TJob : IJob
        {
            IJobDetail job = JobBuilder.Create<TJob>()
                .WithIdentity(typeof(TJob).Name)
                .Build();

            TriggerBuilder triggerBuilder = TriggerBuilder.Create().WithIdentity(typeof(TJob).Name);
            ITrigger trigger = action(triggerBuilder).Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public class JobFactory : IJobFactory
        {
            private readonly IServiceScopeFactory serviceScopeFactory;

            private ConcurrentDictionary<int, IServiceScope> scopes;

            public JobFactory(IServiceScopeFactory serviceScopeFactory)
            {
                this.serviceScopeFactory = serviceScopeFactory;
                this.scopes = new ConcurrentDictionary<int, IServiceScope>();
            }

            public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
            {
                IServiceScope serviceScope = serviceScopeFactory.CreateScope();

                IJob job = serviceScope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
                scopes.TryAdd(job.GetHashCode(), serviceScope);

                return job;
            }

            public void ReturnJob(IJob job)
            {
                int hashcode = job.GetHashCode();
                if (scopes.ContainsKey(hashcode))
                {
                    try
                    {
                        scopes[hashcode].Dispose();
                    }
                    finally
                    {
                        scopes.TryRemove(hashcode, out IServiceScope scope);
                    }
                }

                (job as IDisposable)?.Dispose();
            }
        }
    }
}
