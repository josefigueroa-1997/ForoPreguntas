﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ForoPreguntas.Models
{
    public partial class FOROPREGUNTASContext : DbContext
    {
        public FOROPREGUNTASContext()
        {
        }

        public FOROPREGUNTASContext(DbContextOptions<FOROPREGUNTASContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<Carrera> Carreras { get; set; } = null!;
        public virtual DbSet<Categoria> Categorias { get; set; } = null!;
        public virtual DbSet<CarreraCategoria> CarreraCategorias { get; set; } = null!;
        public virtual DbSet<Usucarrcat> Usucarrcats { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIOS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Contraseña)
                    .IsUnicode(false)
                    .HasColumnName("CONTRASEÑA");

                entity.Property(e => e.Correo)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("CORREO");

                entity.Property(e => e.Imagen).HasColumnName("IMAGEN");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");

                entity.Property(e => e.Salt)
                    .IsUnicode(false)
                    .HasColumnName("SALT");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("TELEFONO");
            });
            modelBuilder.Entity<Carrera>(entity =>
            {
                entity.ToTable("CARRERA");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");

            });
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("CATEGORIA");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");

            });
            modelBuilder.Entity<CarreraCategoria>(entity =>
            {
                entity.ToTable("CARRERA_CATEGORIA");

                entity.HasKey(e => new { e.ID_CARRERA, e.ID_CATEGORIA });

                entity.HasOne(cc => cc.Carrera)
                    .WithMany(c => c.CarreraCategorias)
                    .HasForeignKey(cc => cc.ID_CARRERA)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CARRERA_FK");

                entity.HasOne(cc => cc.Categoria)
                    .WithMany(cat => cat.CarreraCategorias)
                    .HasForeignKey(cc => cc.ID_CATEGORIA)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CATEGORIA_FK");
            });
            modelBuilder.Entity<Usucarrcat>(entity =>
            {
                entity.ToTable("USU_CARR_CAT");

                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ID_USUARIO).IsRequired();

                entity.Property(e => e.ID_CARRERA).IsRequired();

                entity.Property(e => e.ID_CATEGORIA).IsRequired();

                entity.HasOne(e => e.CarreraCategoria)
                    .WithMany()
                    .HasForeignKey(e => new { e.ID_CARRERA, e.ID_CATEGORIA })
                    .HasConstraintName("CARRERAS_FK")
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.ID_USUARIO)
                    .HasConstraintName("USUARIO_FK")
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
