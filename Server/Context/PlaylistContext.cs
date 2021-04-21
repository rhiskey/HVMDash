using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using vkaudioposter_ef.parser;

namespace WAAuth.Server.Context
{
    public class PlaylistContext : DbContext
    {
        public PlaylistContext() { }
        public PlaylistContext(DbContextOptions<PlaylistContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Playlist> Playlists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
 
        }

    }
}
