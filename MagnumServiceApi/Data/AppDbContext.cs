using Microsoft.EntityFrameworkCore;
using MagnumServiceApi.Models;

namespace MagnumServiceApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Move> Moves { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Round> Rounds { get; set; }

        public DbSet<Game> Game {get; set;}
    }
}
