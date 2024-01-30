using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace mvc_dotnet.Models
{
    public partial class TicketServiceContext : DbContext
    {
        public TicketServiceContext()
        {
        }

        public TicketServiceContext(DbContextOptions<TicketServiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Cinema> Cinemas { get; set; } = null!;
        public virtual DbSet<ContentAdmin> ContentAdmins { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Movie> Movies { get; set; } = null!;
        public virtual DbSet<Provole> Provoles { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DIM-LAPTOP\\SQLEXPRESS;Database=tickets_service;Trusted_Connection=True;Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.UserUsernameNavigation)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.UserUsername)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_admins_user");
            });

            modelBuilder.Entity<Cinema>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<ContentAdmin>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.UserUsernameNavigation)
                    .WithMany(p => p.ContentAdmins)
                    .HasForeignKey(d => d.UserUsername)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_content_admin_user");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.UserUsernameNavigation)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.UserUsername)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Table_1_user");
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Name });

                entity.HasOne(d => d.ContentAdmin)
                    .WithMany(p => p.Movies)
                    .HasForeignKey(d => d.ContentAdminId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_movies_content_admin");
            });

            modelBuilder.Entity<Provole>(entity =>
            {
                entity.HasKey(e => new { e.CinemasId, e.MoviesId, e.MoviesName });

                entity.HasOne(d => d.Cinemas)
                    .WithMany(p => p.Provoles)
                    .HasForeignKey(d => d.CinemasId)
                    .HasConstraintName("FK_prov_cin");

                entity.HasOne(d => d.ContentAdmin)
                    .WithMany(p => p.Provoles)
                    .HasForeignKey(d => d.ContentAdminId)
                    .HasConstraintName("FK_prov_content_admin");

                entity.HasOne(d => d.Movies)
                    .WithMany(p => p.Provoles)
                    .HasForeignKey(d => new { d.MoviesId, d.MoviesName })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_prov_movies");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Customers)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.CustomersId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_resrv_custom");

                entity.HasOne(d => d.Provoles)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => new { d.ProvolesCinemasId, d.ProvolesMoviesId, d.ProvolesMoviesName })
                    .HasConstraintName("FK_reservations_provoles");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
