using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IPTVChannelListProxy.Database
{
    public class PlaylistChannel
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }

        public Channel Channel { get; set; }

        public EPGChannel EPG { get; set; }
    }
}
