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
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<ClassModel> Classes { get; set; }
        public virtual DbSet<Time> Time { get; set; }
        public virtual DbSet<Weekday> Weekdays { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Cross> Cross { get; set; }
        public virtual DbSet<Instructor> Instructors { get; set; }
    }
}

