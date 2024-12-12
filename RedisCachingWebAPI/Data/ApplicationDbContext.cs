using Microsoft.EntityFrameworkCore;
using RedisCachingWebAPI.Models;

namespace RedisCachingWebAPI.Data;

public class ApplicationDbContext:DbContext
{
    public DbSet<Driver> Drivers { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
        
    }
}
