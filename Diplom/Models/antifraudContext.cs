using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Diplom
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

        public virtual DbSet<Browser> Browsers { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormTime> FormTimes { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Name> Names { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<SectionTime> SectionTimes { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Surname> Surnames { get; set; }
        public virtual DbSet<System> Systems { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=WIN-7GOJRAHKR5H\\SQLEXPRESS01;Database=antifraud;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Browser>(entity =>
            {
                entity.ToTable("Browser");

                entity.Property(e => e.BrowserId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Device");

                entity.Property(e => e.DeviceId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
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

            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("Language");

                entity.Property(e => e.LanguageId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");

                entity.Property(e => e.LocationId).ValueGeneratedNever();

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(100);
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

            modelBuilder.Entity<Provider>(entity =>
            {
                entity.ToTable("Provider");

                entity.Property(e => e.ProviderId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
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

                entity.Property(e => e.FinishTime).HasColumnType("datetime");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.Value).HasDefaultValueSql("((0))");

                entity.Property(e => e.Vpn).HasColumnName("VPN");

                entity.HasOne(d => d.BrowserNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Browser)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Session_Browser");

                entity.HasOne(d => d.DeviceNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Device)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Session_Device");

                entity.HasOne(d => d.FormNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Form)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Session_FormTime");

                entity.HasOne(d => d.LanguageNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Language)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Session_Language");

                entity.HasOne(d => d.LocationNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Location)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Session_Location");

                entity.HasOne(d => d.ProviderNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Provider)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Session_Provider");

                entity.HasOne(d => d.SectionNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.Section)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Session_SectionTime");

                entity.HasOne(d => d.SystemNavigation)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.System)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Session_System");

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

            modelBuilder.Entity<System>(entity =>
            {
                entity.ToTable("System");

                entity.Property(e => e.SystemId).ValueGeneratedNever();

                entity.Property(e => e.ComputerAbility)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Patronymic)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
