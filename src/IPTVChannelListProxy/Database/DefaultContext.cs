using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPTVChannelListProxy.Database
{
    public class DefaultContext : DbContext
    {
        public DbSet<Channel> Channels { get; set; }

        public DbSet<M3USource> M3USources { get; set; }

        public DbSet<Playlist> Playlists { get; set; }

        public DbSet<PlaylistChannel> PlaylistChannels { get; set; }

        public DbSet<EPGSource> EPGSources { get; set; }

        public DbSet<EPGChannel> EPGChannels { get; set; }

        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
        {
        }
    }
}
