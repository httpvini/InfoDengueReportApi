using Microsoft.EntityFrameworkCore;
using InfoDengueReportAPI.Models;

namespace InfoDengueReportAPI.Data
{
    public class InfoDengueContext(DbContextOptions<InfoDengueContext> options) : DbContext(options)
    {
        public required DbSet<Solicitante> Solicitantes { get; set; }
        public required DbSet<Relatorio> Relatorios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Solicitante>()
                .HasIndex(s => s.Cpf)
                .IsUnique();

            modelBuilder.Entity<Relatorio>()
                .HasOne(r => r.Solicitante)
                .WithMany()
                .HasForeignKey(r => r.SolicitanteId);
        }
    }
}
