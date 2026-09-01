using Microsoft.EntityFrameworkCore;
using SMS.Domain;

namespace SMS.Infrastructure.Persistence;

public class SmsDbContext : DbContext
{
    public SmsDbContext(DbContextOptions<SmsDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmsDbContext).Assembly);

        if (Database.IsNpgsql())
        {
            // Student.DateOfBirth is date-only and carries DateTimeKind.Unspecified. Npgsql's default
            // mapping is "timestamp with time zone", which rejects any Kind other than Utc at write time,
            // so map it to the semantically correct "date" column instead.
            modelBuilder.Entity<Student>().Property(x => x.DateOfBirth).HasColumnType("date");
        }
    }
}
