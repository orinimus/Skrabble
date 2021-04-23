using Microsoft.EntityFrameworkCore;
using SkrabbleLt.InitialData;
using SkrabbleLt.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace SkrabbleLt.Database
{
    public class ScrabbleContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<BoardCell> BoardCells { get; set; }
        public DbSet<Tile> Tiles { get; set; }
        public DbSet<Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = ScrabbleDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tile>().HasData(TileInitialData.DataSeed);
            modelBuilder.Entity<BoardCell>().HasData(BoardInitialData.DataSeed);
        }
    }
}
