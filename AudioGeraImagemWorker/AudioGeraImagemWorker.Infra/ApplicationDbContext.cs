using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Infra.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AudioGeraImagemWorker.Infra
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Comando> Comandos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ComandoConfiguration());
        }
    }
}