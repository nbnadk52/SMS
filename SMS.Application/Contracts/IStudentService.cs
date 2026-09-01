using SMS.Application.Dtos;

namespace SMS.Application.Contracts;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StudentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StudentDto> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);
    Task<StudentDto?> UpdateAsync(Guid id, UpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
