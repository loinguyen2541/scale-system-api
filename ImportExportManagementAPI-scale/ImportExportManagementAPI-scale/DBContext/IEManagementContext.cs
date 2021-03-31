using System;
using ImportExportManagementAPI_scale.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ImportExportManagementAPI_scale.DBContext
{
    public partial class IEManagementContext : DbContext
    {
        public IEManagementContext()
        {
        }

        public IEManagementContext(DbContextOptions<IEManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Goods> Goods { get; set; }
        public virtual DbSet<IdentityCard> IdentityCards { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<InventoryDetail> InventoryDetails { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<PartnerType> PartnerTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<SystemConfig> SystemConfigs { get; set; }
        public virtual DbSet<TimeTemplate> TimeTemplates { get; set; }
        public virtual DbSet<TimeTemplateItem> TimeTemplateItems { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=tcp:iemanagement.database.windows.net,1433;Initial Catalog=IEManagement;Persist Security Info=False;User ID=loinv;Password=123@Admin;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Username);

                entity.ToTable("Account");

                entity.HasIndex(e => e.RoleId, "IX_Account_RoleId");

                entity.Property(e => e.Username).HasMaxLength(25);

                entity.Property(e => e.Password).HasMaxLength(25);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<Goods>(entity =>
            {
                entity.HasKey(e => e.GoodsId);

                entity.Property(e => e.GoodName).HasMaxLength(50);
            });

            modelBuilder.Entity<IdentityCard>(entity =>
            {
                entity.ToTable("IdentityCard");

                entity.HasIndex(e => e.PartnerId, "IX_IdentityCard_PartnerId");

                entity.Property(e => e.IdentityCardId).HasMaxLength(25);

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.IdentityCards)
                    .HasForeignKey(d => d.PartnerId);
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("Inventory");

                entity.Property(e => e.RecordedDate).HasColumnType("date");
            });

            modelBuilder.Entity<InventoryDetail>(entity =>
            {
                entity.ToTable("InventoryDetail");

                entity.HasIndex(e => e.GoodsId, "IX_InventoryDetail_GoodsId");

                entity.HasIndex(e => e.InventoryId, "IX_InventoryDetail_InventoryId");

                entity.HasIndex(e => e.PartnerId, "IX_InventoryDetail_PartnerId");

                entity.HasOne(d => d.Goods)
                    .WithMany(p => p.InventoryDetails)
                    .HasForeignKey(d => d.GoodsId);

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.InventoryDetails)
                    .HasForeignKey(d => d.InventoryId);

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.InventoryDetails)
                    .HasForeignKey(d => d.PartnerId);
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("Partner");

                entity.HasIndex(e => e.PartnerTypeId, "IX_Partner_PartnerTypeId");

                entity.HasIndex(e => e.Username, "IX_Partner_Username")
                    .IsUnique()
                    .HasFilter("([Username] IS NOT NULL)");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.DisplayName).HasMaxLength(200);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(10);

                entity.Property(e => e.Username).HasMaxLength(25);

                entity.HasOne(d => d.PartnerType)
                    .WithMany(p => p.Partners)
                    .HasForeignKey(d => d.PartnerTypeId);

                entity.HasOne(d => d.UsernameNavigation)
                    .WithOne(p => p.Partner)
                    .HasForeignKey<Partner>(d => d.Username);
            });

            modelBuilder.Entity<PartnerType>(entity =>
            {
                entity.ToTable("PartnerType");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleName).HasMaxLength(25);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.HasIndex(e => e.GoodsId, "IX_Schedule_GoodsId");

                entity.HasIndex(e => e.PartnerId, "IX_Schedule_PartnerId");

                entity.HasIndex(e => e.TimeTemplateItemId, "IX_Schedule_TimeTemplateItemId");

                entity.HasOne(d => d.Goods)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.GoodsId);

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.PartnerId);

                entity.HasOne(d => d.TimeTemplateItem)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.TimeTemplateItemId);
            });

            modelBuilder.Entity<SystemConfig>(entity =>
            {
                entity.HasKey(e => e.AttributeKey);

                entity.ToTable("SystemConfig");
            });

            modelBuilder.Entity<TimeTemplate>(entity =>
            {
                entity.ToTable("TimeTemplate");

                entity.Property(e => e.ApplyingDate).HasColumnType("date");

                entity.Property(e => e.TimeTemplateName).HasMaxLength(25);
            });

            modelBuilder.Entity<TimeTemplateItem>(entity =>
            {
                entity.ToTable("TimeTemplateItem");

                entity.HasIndex(e => e.TimeTemplateId, "IX_TimeTemplateItem_TimeTemplateId");

                entity.HasOne(d => d.TimeTemplate)
                    .WithMany(p => p.TimeTemplateItems)
                    .HasForeignKey(d => d.TimeTemplateId);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.HasIndex(e => e.GoodsId, "IX_Transaction_GoodsId");

                entity.HasIndex(e => e.IdentityCardId, "IX_Transaction_IdentityCardId");

                entity.HasIndex(e => e.PartnerId, "IX_Transaction_PartnerId");

                entity.Property(e => e.IdentityCardId).HasMaxLength(25);

                entity.Property(e => e.IsScheduled)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.TimeOut).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.HasOne(d => d.Goods)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.GoodsId);

                entity.HasOne(d => d.IdentityCard)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.IdentityCardId);

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.PartnerId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
