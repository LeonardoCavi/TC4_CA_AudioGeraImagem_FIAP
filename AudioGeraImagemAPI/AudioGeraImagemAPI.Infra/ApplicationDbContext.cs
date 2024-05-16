using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Infra.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AudioGeraImagemAPI.Infra
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Criacao> Criacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CriacaoConfiguration());
        }
    }
}
