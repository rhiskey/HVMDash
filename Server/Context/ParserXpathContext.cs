using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using vkaudioposter_ef.Model;
using vkaudioposter_ef.parser;

namespace WAAuth.Server.Context
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
