using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPTVChannelListProxy.Database;
using IPTVChannelListProxy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPTVChannelListProxy.Controllers
{
    public class PlaylistController : Controller
    {
        private DefaultContext defaultContext;

        public PlaylistController(DefaultContext defaultContext)
        {
            this.defaultContext = defaultContext;
        }

        public IActionResult Index()
        {
            PlaylistsViewModel model = new PlaylistsViewModel();
            model.Playlists = defaultContext.Playlists.OrderBy(p => p.Name).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name)
        {
            await defaultContext.Playlists.AddAsync(new Playlist
            {
                Name = name
            });
            await defaultContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddChannel(int playlist, string name)
        {
            Playlist playlistObj = await defaultContext.Playlists.FirstOrDefaultAsync(p => p.ID == playlist);
            playlistObj.Channels.Add(new PlaylistChannel
            {
                Name = name
            });
            await defaultContext.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = playlist });
        }

        public async Task<IActionResult> Details(int id)
        {
            Playlist playlist = await defaultContext
                .Playlists
                .Include(p => p.Channels)
                .ThenInclude(c => c.Channel)
                .Include(p => p.Channels)
                .ThenInclude(c => c.EPG)
                .FirstOrDefaultAsync(p => p.ID == id);

            return View(playlist);
        }

        [HttpGet("/playlists/{name}.m3u")]
        public async Task<IActionResult> GenerateM3U(string name)
        {
            Playlist playlist = await defaultContext
                .Playlists
                .Include(p => p.Channels)
                .ThenInclude(c => c.Channel)
                .FirstOrDefaultAsync(p => p.Name.ToUpper() == name.ToUpper());

            StringBuilder data = new StringBuilder();
            data.AppendLine("#EXTM3U");

            foreach(PlaylistChannel channel in playlist.Channels)
            {
                data.AppendLine($"#EXTINF:-1 tvg-id=\"{channel.Channel.ChannelID}\" tvg-name=\"{channel.Name}\" tvg-logo=\"{channel.Channel.Logo}\",{channel.Name}");
                data.AppendLine(channel.Channel.Url);
            }

            byte[] codeBytes = Encoding.UTF8.GetBytes(data.ToString());
            FileContentResult fileResult = new FileContentResult(codeBytes, "application/octet-stream");
            fileResult.FileDownloadName = $"{playlist.Name}.m3u";

            return fileResult;
        }
    }
}