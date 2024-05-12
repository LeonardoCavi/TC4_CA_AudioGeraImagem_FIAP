using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Infra.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagemWorker.Infra
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Criacao> Criacoes { get; set; }

        [ExcludeFromCodeCoverage]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CriacaoConfiguration());
        }
    }
}