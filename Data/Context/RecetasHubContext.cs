using System;
using System.Collections.Generic;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Context;

public partial class RecetasHubContext : DbContext
{
    public RecetasHubContext()
    {
    }

    public RecetasHubContext(DbContextOptions<RecetasHubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SecretsSetting> SecretsSettings { get; set; }

    public virtual DbSet<Source> Sources { get; set; }

    public virtual DbSet<SourceItem> SourceItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFavorite> UserFavorites { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\MSSQLSERVER02;Database=RecetasHub;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07C7053205");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SecretsSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SecretsS__3214EC0758DAA775");

            entity.HasIndex(e => e.KeyName, "UQ__SecretsS__F0A2A33788A0FCCE").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.KeyName).HasMaxLength(100);
            entity.Property(e => e.KeyValue).HasMaxLength(500);
        });

        modelBuilder.Entity<Source>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sources__3214EC07A6091CE4");

            entity.Property(e => e.ComponentType).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<SourceItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SourceIt__3214EC070CDAF07F");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Source).WithMany(p => p.SourceItems)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK__SourceIte__Sourc__534D60F1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07AB68F56F");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E458738671").IsUnique();

            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__4D94879B");
        });

        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SourceItemId }).HasName("PK__UserFavo__68733C6C5E251316");

            entity.Property(e => e.SavedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.SourceItem).WithMany(p => p.UserFavorites)
                .HasForeignKey(d => d.SourceItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFavor__Sourc__5AEE82B9");

            entity.HasOne(d => d.User).WithMany(p => p.UserFavorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFavor__UserI__59FA5E80");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
