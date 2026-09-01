using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SMS.Application.Dtos;
using SMS.Application.Services;
using SMS.Infrastructure.Persistence;
using SMS.Infrastructure.Persistence.Repositories;

namespace SMS.UnitTests;

public class StudentServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<SmsDbContext> _options;

    public StudentServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<SmsDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new SmsDbContext(_options);
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }

    private SmsDbContext CreateContext() => new(_options);

    private StudentService CreateService(SmsDbContext context) => new(new StudentRepository(context));

    private static CreateStudentRequest NewStudentRequest(string email = "ada@example.com") => new()
    {
        FirstName = "Ada",
        LastName = "Lovelace",
        Email = email,
        DateOfBirth = new DateTime(2000, 5, 20),
        PhoneNumber = "9800000000"
    };

    [Fact]
    public async Task CreateAsync_WhenValidStudentProvided_ReturnsCreatedStudent()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var created = await service.CreateAsync(NewStudentRequest());

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Ada", created.FirstName);
        Assert.Equal("Lovelace", created.LastName);
        Assert.Equal("ada@example.com", created.Email);
        Assert.Null(created.UpdatedAt);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_PersistsStudentAcrossContexts()
    {
        Guid createdId;

        using (var writeContext = CreateContext())
        {
            var created = await CreateService(writeContext).CreateAsync(NewStudentRequest());
            createdId = created.Id;
        }

        using var readContext = CreateContext();
        var reloaded = await CreateService(readContext).GetByIdAsync(createdId);

        Assert.NotNull(reloaded);
        Assert.Equal(createdId, reloaded!.Id);
        Assert.Equal("ada@example.com", reloaded.Email);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ThrowsDbUpdateException()
    {
        using (var firstContext = CreateContext())
        {
            await CreateService(firstContext).CreateAsync(NewStudentRequest());
        }

        using var secondContext = CreateContext();
        var service = CreateService(secondContext);

        await Assert.ThrowsAsync<DbUpdateException>(() => service.CreateAsync(NewStudentRequest()));
    }

    [Fact]
    public async Task UpdateAsync_WhenStudentExists_UpdatesStudentValues()
    {
        using var context = CreateContext();
        var service = CreateService(context);
        var created = await service.CreateAsync(NewStudentRequest());

        var updated = await service.UpdateAsync(created.Id, new UpdateStudentRequest
        {
            FirstName = "Grace",
            LastName = "Hopper",
            Email = "grace@example.com",
            DateOfBirth = new DateTime(1999, 1, 2),
            PhoneNumber = "9811111111"
        });

        Assert.NotNull(updated);
        Assert.Equal("Grace", updated!.FirstName);
        Assert.Equal("Hopper", updated.LastName);
        Assert.Equal("grace@example.com", updated.Email);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_WhenStudentDoesNotExist_ReturnsNull()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var updated = await service.UpdateAsync(Guid.NewGuid(), new UpdateStudentRequest
        {
            FirstName = "Grace",
            LastName = "Hopper",
            Email = "grace@example.com",
            DateOfBirth = new DateTime(1999, 1, 2)
        });

        Assert.Null(updated);
    }

    [Fact]
    public async Task DeleteAsync_WhenStudentExists_RemovesStudent()
    {
        using var context = CreateContext();
        var service = CreateService(context);
        var created = await service.CreateAsync(NewStudentRequest());

        var deleted = await service.DeleteAsync(created.Id);

        Assert.True(deleted);
        Assert.Null(await service.GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task GetAllAsync_WhenStudentsExist_ReturnsEveryStudent()
    {
        using var context = CreateContext();
        var service = CreateService(context);
        await service.CreateAsync(NewStudentRequest("ada@example.com"));
        await service.CreateAsync(NewStudentRequest("grace@example.com"));

        var students = await service.GetAllAsync();

        Assert.Equal(2, students.Count());
    }
}
