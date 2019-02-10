using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPTVChannelListProxy.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPTVChannelListProxy.Controllers
{
    [Route("api/channel")]
    [ApiController]
    public class ChannelAPIController : ControllerBase
    {
        private readonly DefaultContext defaultContext;

        public ChannelAPIController(DefaultContext defaultContext)
        {
            this.defaultContext = defaultContext;
        }

        [HttpPost("search")]
        public IActionResult Search([FromForm] string q)
        {
            List<Channel> foundChannels = defaultContext
                .Channels
                .Where(c => c.Name.ToUpper().Contains(q.ToUpper()))
                .OrderBy(c => c.Name)
                .ToList();

            return Ok(foundChannels);
        }

        [HttpPost("setchannel")]
        public async Task<IActionResult> SetChannel([FromForm] int playlistChannel, [FromForm] int channel)
        {
            try
            {
                PlaylistChannel pc = defaultContext.PlaylistChannels.FirstOrDefault(x => x.ID == playlistChannel);
                Channel c = defaultContext.Channels.FirstOrDefault(x => x.ID == channel);

                pc.Channel = c;
                await defaultContext.SaveChangesAsync();
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }
    }
}