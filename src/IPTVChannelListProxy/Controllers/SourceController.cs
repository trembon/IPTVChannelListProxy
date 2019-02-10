using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPTVChannelListProxy.Database;
using IPTVChannelListProxy.ScheduledJobs;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace IPTVChannelListProxy.Controllers
{
    public class SourceController : Controller
    {
        private readonly IScheduler scheduler;
        private readonly DefaultContext defaultContext;

        public SourceController(IScheduler scheduler, DefaultContext defaultContext)
        {
            this.scheduler = scheduler;
            this.defaultContext = defaultContext;
        }

        [HttpGet]
        public async Task<IActionResult> Rescan()
        {
            JobKey jobKey = JobKey.Create(typeof(UpdateDataScheduledJob).Name);
            if (await scheduler.CheckExists(jobKey))
                await scheduler.TriggerJob(jobKey);

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> AddM3U(string name, string url)
        {
            await defaultContext.AddAsync(new M3USource
            {
                Name = name,
                Url = url
            });

            await defaultContext.SaveChangesAsync();
            return Redirect("/");
        }
    }
}