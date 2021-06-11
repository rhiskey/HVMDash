using Microsoft.EntityFrameworkCore;
using vkaudioposter_ef.parser;

namespace HVMDash.Server.Context
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
            //optionsBuilder.LogTo(Console.WriteLine);
            //optionsBuilder.EnableSensitiveDataLogging(true);
        }

    }
}