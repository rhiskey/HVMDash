using Microsoft.EntityFrameworkCore;
using vkaudioposter_ef.parser;

namespace HVMDash.Server.Context
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
