using Microsoft.EntityFrameworkCore;
using SMS.Application.Contracts;
using SMS.Domain;

namespace SMS.Infrastructure.Persistence.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(SmsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim();

        return await DbSet
            .AsNoTracking()
            .AnyAsync(x => x.Email == normalized && (excludeId == null || x.Id != excludeId), cancellationToken);
    }
}
