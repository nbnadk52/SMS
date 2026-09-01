using SMS.Domain.Common;

namespace SMS.Domain;

public class Student : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public string? PhoneNumber { get; private set; }

    private Student() { }

    public Student(Guid id, string firstName, string lastName, string email, DateTime dateOfBirth, string? phoneNumber = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.", nameof(lastName));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        if (dateOfBirth > DateTime.Today) throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        DateOfBirth = dateOfBirth.Date;
        PhoneNumber = phoneNumber?.Trim();
    }

    public void Update(string firstName, string lastName, string email, DateTime dateOfBirth, string? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.", nameof(lastName));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        if (dateOfBirth > DateTime.Today) throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        DateOfBirth = dateOfBirth.Date;
        PhoneNumber = phoneNumber?.Trim();
        MarkUpdated();
    }
}
