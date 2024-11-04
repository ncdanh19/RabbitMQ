using Microsoft.EntityFrameworkCore;
using RabbitMQ.Model;

namespace RabbitMQ.Data
{
    public class RabbitDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public RabbitDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("RabbitDbConnection"));
        }

        public DbSet<Product> Products { get; set; }
    }
}
