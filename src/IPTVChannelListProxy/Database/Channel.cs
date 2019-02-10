using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IPTVChannelListProxy.Database
{
    public class Channel
    {
        [Key]
        public int ID { get; set; }

        public string ChannelID { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }

        public string Logo { get; set; }

        public string Url { get; set; }

        public M3USource Source { get; set; }
    }
}
