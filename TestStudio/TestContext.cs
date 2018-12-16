using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestStudio.DTO;

namespace TestStudio
{
    public partial class TestContext : DbContext
    {
        static string _conString = "";

        public TestContext(string ConnectionStrings)
        {
            _conString = ConnectionStrings;
        }

        #region DbSet

        public virtual DbSet<Entities.Test> Test { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_conString, x => x.MigrationsHistoryTable("MigrationsHistory", "Test"));
                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseLoggerFactory(new LoggerFactory(new[] { new DebugLoggerProvider() }));
            }
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Test>(entity =>
            {
                entity.ToTable("Test", "Test");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Json)
                    .IsRequired()
                    .HasColumnName("json")
                    .HasColumnType("jsonb");

                entity.Property(e => e.Json2)
                    .IsRequired()
                    .HasColumnName("json2")
                    .HasColumnType("jsonb");
            });

            modelBuilder.Entity<Entities.Test>().HasData(new Entities.Test()
            {
                Id = Guid.Parse("086B558F-BB80-8645-BC05-27EC1B17B005"),
                Json = JsonConvert.SerializeObject(new Jason() { Str = "1234", Num = 456, Fecha = new Fecha() { Date = DateTime.MinValue }, Logico = true }),
                Json2 = JsonConvert.SerializeObject(new Jason() { Str = "1234", Num = 456, Fecha = new Fecha() { Date = DateTime.MinValue }, Logico = true })
            });

            modelBuilder.Entity<Entities.Test>().HasData(new Entities.Test()
            {
                Id = Guid.Parse("6A250B45-F183-844A-A5DD-FEB33BE7F250"),
                Json = JsonConvert.SerializeObject(new Jason() { Str = "0456", Num = 789, Fecha = new Fecha() { Date = DateTime.MaxValue }, Logico = false }),
                Json2 = JsonConvert.SerializeObject(new Jason() { Str = "0456", Num = 789, Fecha = new Fecha() { Date = DateTime.MaxValue }, Logico = false })
            });
        }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TestContext>
    {
        public TestContext CreateDbContext(string[] args)
        {

            var conStr = "Host=jarvis.local;Port=5434;Database=SIR;Username=SIR;Password=Ab123456";

            return new TestContext(conStr);
        }

        public TestContext CreateDbContext(string connectionString)
        {
            return new TestContext(connectionString);
        }
    }
}
