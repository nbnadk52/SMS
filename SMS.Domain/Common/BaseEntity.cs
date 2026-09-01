namespace SMS.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected BaseEntity() { }

    protected BaseEntity(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Entity id is required.", nameof(id));

        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    protected void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
