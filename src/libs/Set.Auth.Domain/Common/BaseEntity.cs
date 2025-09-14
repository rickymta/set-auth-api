namespace Set.Auth.Domain.Common;

/// <summary>
/// Base entity class with common properties for all entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Created at UTC timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Updated at UTC timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Auditable entity class that extends BaseEntity with auditing properties
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// Created by user identifier
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Updated by user identifier
    /// </summary>
    public string? UpdatedBy { get; set; }
}
