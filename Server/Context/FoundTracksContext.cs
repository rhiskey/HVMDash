using Microsoft.EntityFrameworkCore;
using vkaudioposter_ef.Model;
using vkaudioposter_ef.parser;

namespace HVMDash.Server.Context
{
    public class FoundTracksContext : DbContext
    {
        public FoundTracksContext() { }
        public FoundTracksContext(DbContextOptions<FoundTracksContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FoundTracks> FoundTracks { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.LogTo(Console.WriteLine);
            //optionsBuilder.EnableSensitiveDataLogging(true);
        }

    }
}