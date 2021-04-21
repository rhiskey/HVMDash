using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using vkaudioposter_ef.parser;

namespace WAAuth.Server.Context
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
