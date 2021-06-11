using Microsoft.EntityFrameworkCore;
using vkaudioposter_ef.parser;

namespace HVMDash.Server.Context
{
    public class ConsolePhotostockContext : DbContext
    {
        public ConsolePhotostockContext() { }
        public ConsolePhotostockContext(DbContextOptions<ConsolePhotostockContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ConsolePhotostock> Photostocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

    }
}
