using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SweetsAndTreats.Models
{
  public class SweetsAndTreatsContext : IdentityDbContext<ApplicationUser>
  {

    public DbSet<Flavor> Flavors { get; set; }
    public DbSet<Treat> Treats { get; set; }
    public DbSet<FlavorTreat> FlavorTreats { get; set; }
    
    public SweetsAndTreatsContext(DbContextOptions options) : base(options) { }
  }
}