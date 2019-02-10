using IPTVChannelListProxy.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPTVChannelListProxy.Models
{
    public class SourcesViewModel
    {
        public List<M3USource> M3USources { get; set; }

        public List<EPGSource> EPGSources { get; set; }
    }
}
