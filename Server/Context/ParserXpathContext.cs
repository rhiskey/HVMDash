using Microsoft.EntityFrameworkCore;
using vkaudioposter_ef.Model;

namespace HVMDash.Server.Context
{
    public class ParserXpathContext : DbContext
    {
        public ParserXpathContext() { }
        public ParserXpathContext(DbContextOptions<ParserXpathContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ParserXpath> ParserXpaths { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

    }
}
