using System;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
	public class CityInfoContext : DbContext
	{
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("connectionstring");
            base.OnConfiguring(optionsBuilder);
        }
    }
}

