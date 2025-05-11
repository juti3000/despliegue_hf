using Microsoft.EntityFrameworkCore;
using Habitforger.Models;
using System.Collections.Generic;

namespace Habitforger.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor que recibe las opciones de configuración del DbContext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet para cada tabla de la base de datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Habito> Habitos { get; set; }
        public DbSet<Logro> Logros { get; set; }
        public DbSet<Estadisticas> Estadisticas { get; set; }

        // Configuración adicional del modelo (opcional)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario"); // Especifica el nombre de la tabla
                entity.HasKey(u => u.IdUsuario); // Define la clave primaria
                entity.Property(u => u.IdUsuario).HasColumnName("id_usuario"); // Mapea la columna

                entity.Property(u => u.NombreUsuario)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Contrasena)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                // Configura las restricciones de unicidad
                entity.HasIndex(u => u.NombreUsuario).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Configuración de relaciones y restricciones
            modelBuilder.Entity<Habito>()
                .HasOne(h => h.Usuario)
                .WithMany(u => u.Habitos)
                .HasForeignKey(h => h.IdUsuario);

            modelBuilder.Entity<Logro>()
                .HasOne(l => l.Usuario)
                .WithMany(u => u.Logros)
                .HasForeignKey(l => l.IdUsuario);

            modelBuilder.Entity<Estadisticas>()
                .HasOne(e => e.Usuario)
                .WithMany(u => u.Estadisticas)
                .HasForeignKey(e => e.IdUsuario);
        }
    }
}