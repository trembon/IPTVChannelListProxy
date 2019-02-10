using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IPTVChannelListProxy.Database
{
    public class M3USource
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime? LastScan { get; set; }

        public List<Channel> Channels { get; set; }

        public M3USource()
        {
            this.Channels = new List<Channel>();
        }
    }
}
