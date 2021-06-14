using Microsoft.EntityFrameworkCore;
using vkaudioposter_ef.Model;

namespace HVMDash.Server.Context
{
    public class ConfigurationContext : DbContext
    {
        public ConfigurationContext() { }
        public ConfigurationContext(DbContextOptions<ConfigurationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Configuration> Configurations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
