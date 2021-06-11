using Microsoft.EntityFrameworkCore;
using vkaudioposter_ef.Model;

namespace HVMDash.Server.Context
{
    public class PostContext : DbContext
    {
        public PostContext() { }
        public PostContext(DbContextOptions<PostContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLazyLoadingProxies(true);

            //optionsBuilder.LogTo(Console.WriteLine);
            //optionsBuilder.EnableSensitiveDataLogging(true);
            //optionsBuilder.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
        }

    }
}
