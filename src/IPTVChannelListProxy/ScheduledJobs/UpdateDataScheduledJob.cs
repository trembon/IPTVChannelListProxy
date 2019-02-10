using IPTVChannelListProxy.Database;
using IPTVChannelListProxy.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IPTVChannelListProxy.ScheduledJobs
{
    public class UpdateDataScheduledJob : IJob
    {
        private readonly ILogger logger;

        private readonly DefaultContext defaultContext;
        private readonly IM3UParserService parserService;

        public UpdateDataScheduledJob(DefaultContext defaultContext, IM3UParserService parserService, ILogger<UpdateDataScheduledJob> logger)
        {
            this.logger = logger;

            this.defaultContext = defaultContext;
            this.parserService = parserService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("Starting...");

            List<M3USource> m3uSources = defaultContext.M3USources.ToList();
            foreach(M3USource source in m3uSources)
            {
                logger.LogInformation($"Processing {source.Name}");

                using(WebClient client = new WebClient())
                {
                    source.LastScan = DateTime.Now;

                    string m3u = await client.DownloadStringTaskAsync(source.Url);
                    List<Channel> channels = parserService.Parse(m3u).ToList();
                    
                    logger.LogInformation($"Processing {source.Name} with {channels.Count} channels");

                    foreach (Channel channel in channels)
                    {
                        Channel existingChannel = defaultContext.Channels.FirstOrDefault(c => c.ChannelID == channel.ChannelID && c.Source.ID == source.ID);
                        if (existingChannel != null)
                        {
                            if (existingChannel.Name != channel.Name)
                                existingChannel.Name = channel.Name;

                            if (existingChannel.Url != channel.Url)
                                existingChannel.Url = channel.Url;

                            if (existingChannel.Logo != channel.Logo)
                                existingChannel.Logo = channel.Logo;
                        }
                        else
                        {
                            channel.Source = source;
                            defaultContext.Channels.Add(channel);
                        }
                    }

                    int changes = await defaultContext.SaveChangesAsync();
                    logger.LogInformation($"Done with {changes}.");
                }
            }
        }
    }
}
