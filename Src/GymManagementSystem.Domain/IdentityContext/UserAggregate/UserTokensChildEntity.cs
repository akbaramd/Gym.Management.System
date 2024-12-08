using Bonyan.Layer.Domain.Entities;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace GymManagementSystem.Domain.IdentityContext.UserAggregate;

/// <summary>
/// Child entity for user-token relationship.
/// </summary>
public class UserTokensChildEntity : BonEntity<Guid>
{
  public Guid UserId { get; private set; }
  public string Type { get; private set; }
  public string Value { get; private set; }

  protected UserTokensChildEntity() { } // EF Core constructor

  public UserTokensChildEntity(Guid userId, string type, string value)
  {
    Id = Guid.NewGuid();
    UserId = userId;
    Type = type ?? throw new ArgumentNullException(nameof(type), "Token type cannot be null.");
    Value = value ?? throw new ArgumentNullException(nameof(value), "Token value cannot be null.");
  }
}
