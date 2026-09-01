using SMS.Domain;

namespace SMS.Application.Contracts;

public interface IStudentRepository : IRepository<Student>
{
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
