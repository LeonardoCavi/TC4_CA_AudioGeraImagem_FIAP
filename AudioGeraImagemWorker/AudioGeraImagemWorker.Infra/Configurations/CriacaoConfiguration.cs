using AudioGeraImagem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioGeraImagemWorker.Infra.Configurations
{
    public class CriacaoConfiguration : IEntityTypeConfiguration<Criacao>
    {
        public void Configure(EntityTypeBuilder<Criacao> builder)
        {
            builder.ToTable("Criacao");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Descricao)
                .HasColumnType("VARCHAR(256)");
            builder.Property(x => x.InstanteCriacao)
                .HasColumnType("DATETIME2");
            builder.Property(x => x.InstanteAtualizacao)
                .HasColumnType("DATETIME2");
            builder.Property(x => x.UrlAudio)
                .HasColumnType("VARCHAR(MAX)")
                .IsRequired(false);
            builder.Property(x => x.Transcricao)
                .HasColumnType("VARCHAR(MAX)")
                .IsRequired(false);
            builder.Property(x => x.UrlImagem)
                .HasColumnType("VARCHAR(MAX)")
                .IsRequired(false);

            builder.OwnsMany(x => x.ProcessamentosCriacao, processamentoCriacao =>
            {
                processamentoCriacao.ToTable("ProcessamentosCriacao");
                processamentoCriacao.HasKey(x => x.Id);
                processamentoCriacao.Property(x => x.Estado).HasConversion<string>()
                    .HasColumnType("VARCHAR(20)");
                processamentoCriacao.Property(x => x.InstanteCriacao)
                    .HasColumnType("DATETIME2");
                processamentoCriacao.Property(x => x.MensagemErro)
                    .HasColumnType("VARCHAR(256)")
                    .IsRequired(false);
            });
        }
    }
}