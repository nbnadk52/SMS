using SMS.Application.Contracts;
using SMS.Application.Dtos;
using SMS.Domain;

namespace SMS.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var students = await _studentRepository.GetAllAsync(cancellationToken);
        return students.Select(MapToDto).ToList();
    }

    public async Task<StudentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
        return student is null ? null : MapToDto(student);
    }

    public async Task<StudentDto> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var student = new Student(Guid.NewGuid(), request.FirstName, request.LastName, request.Email, request.DateOfBirth, request.PhoneNumber);

        await _studentRepository.AddAsync(student, cancellationToken);
        await _studentRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(student);
    }

    public async Task<StudentDto?> UpdateAsync(Guid id, UpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            return null;
        }

        student.Update(request.FirstName, request.LastName, request.Email, request.DateOfBirth, request.PhoneNumber);

        _studentRepository.Update(student);
        await _studentRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(student);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            return false;
        }

        _studentRepository.Remove(student);
        await _studentRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static StudentDto MapToDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth,
            PhoneNumber = student.PhoneNumber,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt
        };
    }
}
