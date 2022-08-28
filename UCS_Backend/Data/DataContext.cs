using System;
using Microsoft.EntityFrameworkCore;
using UCS_Backend.Models;

namespace UCS_Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Individual> Individuals { get; set; }
    }
}

