using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using vkaudioposter_ef.parser;

namespace WAAuth.Server.Context
{
    public class PostedTracksContext : DbContext
    {
        public PostedTracksContext() { }
        public PostedTracksContext(DbContextOptions<PostedTracksContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PostedTrack> PostedTracks { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

    }
}