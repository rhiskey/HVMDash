using Microsoft.EntityFrameworkCore;
using vkaudioposter_ef.Model;

namespace HVMDash.Server.Context
{
    public class VKAccountsContext : DbContext
    {
        public VKAccountsContext() { }
        public VKAccountsContext(DbContextOptions<VKAccountsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<VKAccounts> VKAccounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
