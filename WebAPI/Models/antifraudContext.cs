using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebAPI
{
    public partial class antifraudContext : DbContext
    {
        public antifraudContext()
        {
        }

        public antifraudContext(DbContextOptions<antifraudContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormTime> FormTimes { get; set; }
        public virtual DbSet<Name> Names { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<SectionTime> SectionTimes { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Surname> Surnames { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Server=WIN-7GOJRAHKR5H\\SQLEXPRESS01;Database=antifraud;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");

                entity.Property(e => e.CountryId).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Form>(entity =>
            {
                entity.ToTable("Form");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<FormTime>(entity =>
            {
                entity.ToTable("FormTime");

                entity.HasOne(d => d.FormNavigation)
                    .WithMany(p => p.FormTimes)
                    .HasForeignKey(d => d.Form)
                    .HasConstraintName("FK_FormTime_Form");
            });

            modelBuilder.Entity<Name>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Name");

                entity.Property(e => e.Name1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Name");
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.ToTable("Section");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<SectionTime>(entity =>
            {
                entity.ToTable("SectionTime");

                entity.HasOne(d => d.SectionNavigation)
                    .WithMany(p => p.SectionTimes)
                    .HasForeignKey(d => d.Section)
                    .HasConstraintName("FK_SectionTime_Section");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("Session");

                entity.Property(e => e.SessionId).ValueGeneratedNever();

                entity.Property(e => e.Pk).HasColumnName("PK");

                entity.HasOne(d => d.CountryNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Country)
                    .HasConstraintName("FK_Session_Country");

                entity.HasOne(d => d.FormNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Form)
                    .HasConstraintName("FK_Session_FormTime");

                entity.HasOne(d => d.SectionNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Section)
                    .HasConstraintName("FK_Session_SectionTime");

                entity.HasOne(d => d.UsersNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Users)
                    .HasConstraintName("FK_Session_Users");
            });

            modelBuilder.Entity<Surname>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Surname");

                entity.Property(e => e.Surname1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Surname");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Fio)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("FIO");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
