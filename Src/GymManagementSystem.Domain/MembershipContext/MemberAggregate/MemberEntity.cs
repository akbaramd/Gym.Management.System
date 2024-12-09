#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace GymManagementSystem.Domain.MembershipContext.MemberAggregate;

/// <summary>
///   Member aggregate root encapsulating membership-related data.
/// </summary>
public class MemberEntity : BonAggregateRoot<Guid>
{
  protected MemberEntity() { } // EF Core constructor

  public MemberEntity(Guid userId, string membershipCode, string? address = null, string? postalCode = null)
  {
    Id = Guid.NewGuid();
    MembershipCode = membershipCode;
    UserId = userId;
    Address = address;
    PostalCode = postalCode;
  }

  public Guid UserId { get; set; }
  public string MembershipCode { get; private set; }
  public string? Address { get; private set; }
  public string? PostalCode { get; private set; }
}
