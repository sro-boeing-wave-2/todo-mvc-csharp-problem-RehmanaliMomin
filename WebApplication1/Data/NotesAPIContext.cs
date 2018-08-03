using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class NotesAPIContext : DbContext
    {
        public NotesAPIContext(DbContextOptions<NotesAPIContext> options)
           : base(options)
        {
        }

        public DbSet<Notes> notes { get; set; }
        //public DbSet<Label> label {get; set;}
        //public DbSet<Content> content { get; set; }

    }
}