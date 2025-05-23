﻿// <auto-generated />
using System;
using Habitforger.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Habitforger.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250327120115_ModificacionLeve1")]
    partial class ModificacionLeve1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Habitforger.Models.Estadisticas", b =>
                {
                    b.Property<int>("IdEstadistica")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("HabitoMayorAvance")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("int");

                    b.Property<int>("NumHabitActivos")
                        .HasColumnType("int");

                    b.Property<int>("NumHabitCompletados")
                        .HasColumnType("int");

                    b.Property<int>("NumHabitTotal")
                        .HasColumnType("int");

                    b.Property<float>("PorcentajeMayorAvance")
                        .HasColumnType("float");

                    b.Property<int>("RachaMaxActNum")
                        .HasColumnType("int");

                    b.Property<string>("RachaMaxActTitulo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("RachaMaxTotNum")
                        .HasColumnType("int");

                    b.Property<string>("RachaMaxTotTitulo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("IdEstadistica");

                    b.HasIndex("IdUsuario");

                    b.ToTable("Estadisticas");
                });

            modelBuilder.Entity("Habitforger.Models.Habito", b =>
                {
                    b.Property<int>("IdHabito")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("DiasCompletados")
                        .HasColumnType("int");

                    b.Property<int>("DiasFallados")
                        .HasColumnType("int");

                    b.Property<int>("DiasPosibles")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Frecuencia")
                        .HasColumnType("int");

                    b.Property<bool>("HacerPrivado")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("HoyRespondido")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("int");

                    b.Property<int>("MaximaRacha")
                        .HasColumnType("int");

                    b.Property<int>("Objetivo")
                        .HasColumnType("int");

                    b.Property<bool>("ObjetivoCumplido")
                        .HasColumnType("tinyint(1)");

                    b.Property<float>("Progreso")
                        .HasColumnType("float");

                    b.Property<int>("RachaActual")
                        .HasColumnType("int");

                    b.Property<int>("RecuentoObjetivo")
                        .HasColumnType("int");

                    b.Property<string>("TituloHabito")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UltimaActualizacion")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("UltimaRespuestaFecha")
                        .HasColumnType("datetime(6)");

                    b.HasKey("IdHabito");

                    b.HasIndex("IdUsuario");

                    b.ToTable("Habitos");
                });

            modelBuilder.Entity("Habitforger.Models.Logro", b =>
                {
                    b.Property<int>("IdLogro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Completado")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("int");

                    b.Property<string>("NombreLogro")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("IdLogro");

                    b.HasIndex("IdUsuario");

                    b.ToTable("Logros");
                });

            modelBuilder.Entity("Habitforger.Models.Usuario", b =>
                {
                    b.Property<int>("IdUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_usuario");

                    b.Property<string>("Contrasena")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("NombreUsuario")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("IdUsuario");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("NombreUsuario")
                        .IsUnique();

                    b.ToTable("usuario", (string)null);
                });

            modelBuilder.Entity("Habitforger.Models.Estadisticas", b =>
                {
                    b.HasOne("Habitforger.Models.Usuario", "Usuario")
                        .WithMany("Estadisticas")
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Habitforger.Models.Habito", b =>
                {
                    b.HasOne("Habitforger.Models.Usuario", "Usuario")
                        .WithMany("Habitos")
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Habitforger.Models.Logro", b =>
                {
                    b.HasOne("Habitforger.Models.Usuario", "Usuario")
                        .WithMany("Logros")
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Habitforger.Models.Usuario", b =>
                {
                    b.Navigation("Estadisticas");

                    b.Navigation("Habitos");

                    b.Navigation("Logros");
                });
#pragma warning restore 612, 618
        }
    }
}
