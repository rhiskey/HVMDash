using Microsoft.EntityFrameworkCore;
using vkaudioposter_ef.Model;

namespace HVMDash.Server.Context
{
    public class TelegramUsersContext : DbContext
    {
        public TelegramUsersContext() { }
        public TelegramUsersContext(DbContextOptions<TelegramUsersContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TelegramUser> TelegramUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
