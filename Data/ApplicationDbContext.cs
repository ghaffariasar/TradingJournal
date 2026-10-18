using TradingProject.Models;
using TradingProject.Models.Enums;
using Microsoft.EntityFrameworkCore;
using TradingProject.Security;

namespace TradingProject.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Trade> Trades { get; set; }
        public DbSet<TradSystem> TradSystems { get; set; }
        public DbSet<Pattern> Patterns { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<TradingSymbol> TradingSymbols { get; set; }
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<TradingAccount> TradingAccounts { get; set; }
        public DbSet<TradeAttachment> TradeAttachments { get; set; }
        public DbSet<TradeTradeSystem> TradeTradeSystems { get; set; }
        public DbSet<TradeTradePattern> TradeTradePatterns { get; set; }
        public DbSet<TradingSession> TradingSessions { get; set; }
        public DbSet<TradeAnalysisOutcome> TradeAnalysisOutcomes { get; set; }
        public DbSet<TradeEmotion> TradeEmotions { get; set; }
        public DbSet<TradeTradeEmotion> TradeTradeEmotions { get; set; }
        public DbSet<AppRole> Roles { get; set; }
        public DbSet<AppPermission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermissionOverride> UserPermissionOverrides { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trade>(b =>
            {
                b.Property(t => t.Direction)
                    .HasConversion<int>();

                b.Property(t => t.Status)
                    .HasConversion<int>()
                    .HasDefaultValue(TradeStatus.Closed);

                b.Property(t => t.Timeframe)
                    .HasConversion<int>();

                b.HasIndex(t => t.UserId);
                b.HasIndex(t => t.Status);
                b.HasIndex(t => t.Direction);
                b.HasIndex(t => t.StartTime);
                b.HasIndex(t => t.TradeSymbol);
                b.HasIndex(t => t.TradingSessionId);
                b.HasIndex(t => t.TradeAnalysisOutcomeId);

                b.HasOne(t => t.User)
                    .WithMany(u => u.Trades)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Restrict);  
              
                b.HasOne(t => t.Symbol)
                    .WithMany(s => s.Trades)
                    .HasForeignKey(t => t.SymbolId)
                    .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(t => t.Broker)
                    .WithMany(b => b.Trades)
                    .HasForeignKey(t => t.BrokerId)
                    .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(t => t.TradingAccount)
                    .WithMany(a => a.Trades)
                    .HasForeignKey(t => t.TradingAccountId)
                    .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(t => t.TradingSession)
                    .WithMany(s => s.Trades)
                    .HasForeignKey(t => t.TradingSessionId)
                    .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(t => t.TradeAnalysisOutcome)
                    .WithMany(o => o.Trades)
                    .HasForeignKey(t => t.TradeAnalysisOutcomeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TradeTradeSystem>(b =>
            {
                b.HasKey(j => new { j.TradeId, j.TradSystemId });

                b.HasOne(j => j.Trade)
                    .WithMany(t => t.TradeTradeSystems)
                    .HasForeignKey(j => j.TradeId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(j => j.TradSystem)
                    .WithMany(s => s.TradeTradeSystems)
                    .HasForeignKey(j => j.TradSystemId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.ToTable("TradeTradSystems");
            });

            modelBuilder.Entity<TradeTradePattern>(b =>
            {
                b.HasKey(j => new { j.TradeId, j.PatternId });

                b.HasOne(j => j.Trade)
                    .WithMany(t => t.TradeTradePatterns)
                    .HasForeignKey(j => j.TradeId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(j => j.Pattern)
                    .WithMany(p => p.TradeTradePatterns)
                    .HasForeignKey(j => j.PatternId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.ToTable("TradeTradePatterns");
            });

            modelBuilder.Entity<TradeAttachment>(b =>
            {
                b.HasOne(a => a.Trade)
                    .WithMany(t => t.Attachments)
                    .HasForeignKey(a => a.TradeId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(a => a.TradeId);
            });

            modelBuilder.Entity<TradingSymbol>(b =>
            {
                b.Property(s => s.InstrumentType)
                    .HasConversion<int>()
                    .HasDefaultValue(InstrumentType.Forex);

                b.HasIndex(s => s.Symbol)
                    .IsUnique();
            });

            modelBuilder.Entity<Broker>(b =>
            {
                b.HasIndex(b => b.Name)
                    .IsUnique();
            });

            modelBuilder.Entity<TradingAccount>(b =>
            {
                b.HasOne(a => a.Broker)
                    .WithMany(b => b.TradingAccounts)
                    .HasForeignKey(a => a.BrokerId)
                    .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(a => a.User)
                    .WithMany(u => u.TradingAccounts)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(a => a.UserId);
            });

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.Role)
                    .HasConversion<int>()
                    .HasDefaultValue(UserRole.Trader);

                b.HasIndex(u => u.Username)
                    .IsUnique();

                b.HasData(new ApplicationUser
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    DisplayName = "Administrator",
                    Role = UserRole.Admin,
                    RoleId = 1,
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            });

            modelBuilder.Entity<TradingSession>(b =>
            {
                b.HasIndex(s => s.Name).IsUnique();
                b.HasData(SeedSessions());
            });

            modelBuilder.Entity<TradeAnalysisOutcome>(b =>
            {
                b.HasIndex(o => o.Name).IsUnique();
                b.HasData(SeedOutcomes());
            });

            modelBuilder.Entity<TradeEmotion>(b =>
            {
                b.Property(e => e.Category).HasConversion<int>();
                b.HasIndex(e => new { e.Category, e.Name }).IsUnique();
                b.HasData(SeedEmotions());
            });

            modelBuilder.Entity<TradeTradeEmotion>(b =>
            {
                b.HasKey(j => new { j.TradeId, j.TradeEmotionId });

                b.Property(j => j.Category).HasConversion<int>();

                b.HasOne(j => j.Trade)
                    .WithMany(t => t.TradeTradeEmotions)
                    .HasForeignKey(j => j.TradeId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(j => j.TradeEmotion)
                    .WithMany(e => e.TradeTradeEmotions)
                    .HasForeignKey(j => j.TradeEmotionId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.ToTable("TradeTradeEmotions");
            });

            modelBuilder.Entity<AppRole>(b =>
            {
                b.HasIndex(r => r.Code).IsUnique();
                b.HasData(SeedRoles());
            });

            modelBuilder.Entity<AppPermission>(b =>
            {
                b.HasIndex(p => p.Code).IsUnique();
                b.HasData(SeedPermissions());
            });

            modelBuilder.Entity<RolePermission>(b =>
            {
                b.HasKey(rp => new { rp.RoleId, rp.PermissionId });
                b.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.RoleId);
                b.HasOne(rp => rp.Permission).WithMany(p => p.RolePermissions).HasForeignKey(rp => rp.PermissionId);
                b.HasData(SeedRolePermissions());
            });

            modelBuilder.Entity<UserPermissionOverride>(b =>
            {
                b.HasKey(upo => new { upo.UserId, upo.PermissionId });
                b.HasOne(upo => upo.User).WithMany(u => u.PermissionOverrides).HasForeignKey(upo => upo.UserId);
                b.HasOne(upo => upo.Permission).WithMany(p => p.UserOverrides).HasForeignKey(upo => upo.PermissionId);
            });

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasOne(u => u.AppRole)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private static AppRole[] SeedRoles() =>
        [
            new() { Id = 1, Code = "Admin", Name = "Administrator", NameFa = "مدیر سیستم", IsSystem = true, CreatedDate = SeedCreated },
            new() { Id = 2, Code = "Trader", Name = "Trader", NameFa = "معامله‌گر", IsSystem = true, CreatedDate = SeedCreated },
            new() { Id = 3, Code = "Operator", Name = "Operator", NameFa = "اپراتور", IsSystem = true, CreatedDate = SeedCreated }
        ];

        private static AppPermission[] SeedPermissions()
        {
            var permissions = new List<(string Code, string Name, string NameFa, string Desc, string DescFa)>
            {
                (PermissionCodes.DashboardView, "View Dashboard", "مشاهده داشبورد", "Access to the main dashboard page", "دسترسی به صفحه اصلی داشبورد"),
                (PermissionCodes.TradesViewOwn, "View Own Trades", "مشاهده معاملات خود", "Ability to see personal trading history", "قابلیت مشاهده تاریخچه معاملات شخصی"),
                (PermissionCodes.TradesCreateOwn, "Create Own Trades", "ثبت معامله جدید", "Ability to add new trades", "قابلیت افزودن معاملات جدید"),
                (PermissionCodes.TradesEditOwn, "Edit Own Trades", "ویرایش معاملات خود", "Ability to modify own trades", "قابلیت ویرایش معاملات شخصی"),
                (PermissionCodes.TradesDeleteOwn, "Delete Own Trades", "حذف معاملات خود", "Ability to remove own trades", "قابلیت حذف معاملات شخصی"),
                (PermissionCodes.ReportsViewOwn, "View Own Reports", "مشاهده گزارش‌های خود", "Access to personal performance reports", "دسترسی به گزارش‌های عملکرد شخصی"),
                (PermissionCodes.AccountsManage, "Manage Accounts", "مدیریت حساب‌های معاملاتی", "Ability to manage personal trading accounts", "قابلیت مدیریت حساب‌های معاملاتی شخصی"),
                (PermissionCodes.LookupsManage, "Manage Lookups", "مدیریت اطلاعات پایه", "Manage symbols, brokers, systems, etc.", "مدیریت نمادها، کارگزاران، سیستم‌ها و غیره"),
                (PermissionCodes.UsersView, "View Users", "مشاهده کاربران", "Ability to see the list of users", "قابلیت مشاهده لیست کاربران"),
                (PermissionCodes.UsersCreateEdit, "Create/Edit Users", "ایجاد/ویرایش کاربر", "Create new users or modify existing ones", "ایجاد کاربر جدید یا ویرایش کاربران موجود"),
                (PermissionCodes.UsersManageAccess, "Manage Permissions", "مدیریت دسترسی‌ها", "Assign roles and specific permissions", "تخصیص نقش‌ها و دسترسی‌های خاص"),
                (PermissionCodes.UsersViewOthersTrades, "View Others' Trades", "مشاهده معاملات دیگران", "Administrator ability to see all users' data", "قابلیت مدیر برای مشاهده داده‌های تمامی کاربران")
            };

            var list = new List<AppPermission>();
            var id = 1;
            foreach (var p in permissions)
            {
                var parts = p.Code.Split('.');
                list.Add(new AppPermission
                {
                    Id = id++,
                    Code = p.Code,
                    Name = p.Name,
                    NameFa = p.NameFa,
                    Module = parts[0],
                    Description = p.Desc,
                    DescriptionFa = p.DescFa,
                    SortOrder = id
                });
            }
            return [.. list];
        }

        private static RolePermission[] SeedRolePermissions()
        {
            var list = new List<RolePermission>();
            var permissions = SeedPermissions();

            // Admin has all
            foreach (var p in permissions)
            {
                list.Add(new RolePermission { RoleId = 1, PermissionId = p.Id });
            }

            // Trader has defaults
            var traderDefaults = Security.PermissionCodes.TraderDefaults;
            foreach (var p in permissions.Where(x => traderDefaults.Contains(x.Code)))
            {
                list.Add(new RolePermission { RoleId = 2, PermissionId = p.Id });
            }

            // Operator has defaults (same as trader for now as requested)
            foreach (var p in permissions.Where(x => traderDefaults.Contains(x.Code)))
            {
                list.Add(new RolePermission { RoleId = 3, PermissionId = p.Id });
            }

            return [.. list];
        }

        private static readonly DateTime SeedCreated = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static TradingSession[] SeedSessions() =>
        [
            new() { Id = 1, Name = "Sydney", NameFa = "سیدنی", Description = "21:00–06:00 UTC", StartTimeOfDay = new TimeSpan(21, 0, 0), EndTimeOfDay = new TimeSpan(6, 0, 0), SortOrder = 1, IsActive = true, CreatedDate = SeedCreated },
            new() { Id = 2, Name = "Tokyo", NameFa = "توکیو", Description = "00:00–09:00 UTC", StartTimeOfDay = new TimeSpan(0, 0, 0), EndTimeOfDay = new TimeSpan(9, 0, 0), SortOrder = 2, IsActive = true, CreatedDate = SeedCreated },
            new() { Id = 3, Name = "London", NameFa = "لندن", Description = "07:00–16:00 UTC", StartTimeOfDay = new TimeSpan(7, 0, 0), EndTimeOfDay = new TimeSpan(16, 0, 0), SortOrder = 3, IsActive = true, CreatedDate = SeedCreated },
            new() { Id = 4, Name = "New York", NameFa = "نیویورک", Description = "12:00–21:00 UTC", StartTimeOfDay = new TimeSpan(12, 0, 0), EndTimeOfDay = new TimeSpan(21, 0, 0), SortOrder = 4, IsActive = true, CreatedDate = SeedCreated },
            new() { Id = 5, Name = "London / New York Overlap", NameFa = "همپوشانی لندن و نیویورک", Description = "12:00–16:00 UTC", StartTimeOfDay = new TimeSpan(12, 0, 0), EndTimeOfDay = new TimeSpan(16, 0, 0), SortOrder = 5, IsActive = true, CreatedDate = SeedCreated }
        ];

        private static TradeAnalysisOutcome[] SeedOutcomes() =>
        [
            new() { Id = 1, Name = "Correct Analysis – Profitable Trade", NameFa = "تحلیل درست – معامله سودده", SortOrder = 1, IsActive = true, CreatedDate = SeedCreated },
            new() { Id = 2, Name = "Correct Analysis – Losing Trade", NameFa = "تحلیل درست – معامله زیان‌ده", SortOrder = 2, IsActive = true, CreatedDate = SeedCreated },
            new() { Id = 3, Name = "Incorrect Analysis – Profitable Trade", NameFa = "تحلیل نادرست – معامله سودده", SortOrder = 3, IsActive = true, CreatedDate = SeedCreated },
            new() { Id = 4, Name = "Incorrect Analysis – Losing Trade", NameFa = "تحلیل نادرست – معامله زیان‌ده", SortOrder = 4, IsActive = true, CreatedDate = SeedCreated }
        ];

        private static TradeEmotion[] SeedEmotions()
        {
            var names = new (string En, string Fa)[]
            {
                ("Experienced severe stress", "استرس شدید را تجربه کردم"),
                ("Price approached the stop-loss, but I remained disciplined", "قیمت به حد ضرر نزدیک شد اما من پایبند ماندم"),
                ("I wish I had closed later", "ای کاش دیرتر می‌بستم"),
                ("I wish I had waited longer", "ای کاش بیشتر صبر می‌کردم")
            };

            var categories = new[] { EmotionCategory.AtEntry, EmotionCategory.DuringTrade, EmotionCategory.AfterClose };
            var list = new List<TradeEmotion>();
            var id = 1;
            foreach (var category in categories)
            {
                var order = 1;
                foreach (var (en, fa) in names)
                {
                    list.Add(new TradeEmotion
                    {
                        Id = id++,
                        Category = category,
                        Name = en,
                        NameFa = fa,
                        SortOrder = order++,
                        IsActive = true,
                        CreatedDate = SeedCreated
                    });
                }
            }

            return [.. list];
        }
    }
}
