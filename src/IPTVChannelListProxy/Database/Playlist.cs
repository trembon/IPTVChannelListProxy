using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IPTVChannelListProxy.Database
{
    public class Playlist
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }

        public List<PlaylistChannel> Channels { get; set; }

        public Playlist()
        {
            this.Channels = new List<PlaylistChannel>();
        }
    }
}
