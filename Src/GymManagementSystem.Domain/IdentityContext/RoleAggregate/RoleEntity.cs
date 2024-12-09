#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace GymManagementSystem.Domain.IdentityContext.RoleAggregate;

/// <summary>
///   Role aggregate root for managing roles.
/// </summary>
public class RoleEntity : BonAggregateRoot<Guid>
{
  private readonly List<RolePermissionChildEntity> _permissions = new();

  private RoleEntity() { } // EF Core constructor

  /// <summary>
  ///   Constructor for creating a new role.
  /// </summary>
  /// <param name="name">Unique name of the role.</param>
  /// <param name="title">Descriptive title of the role.</param>
  public RoleEntity(string name, string title)
  {
    Id = Guid.NewGuid();
    Name = name ?? throw new ArgumentNullException(nameof(name));
    Title = title ?? throw new ArgumentNullException(nameof(title));

    AddDomainEvent(new RoleCreatedEvent(Id, Name, Title));
  }

  public string Name { get; private set; }
  public string Title { get; private set; }
  public IReadOnlyCollection<RolePermissionChildEntity> Permissions => _permissions;

  /// <summary>
  ///   Adds a permission to the role.
  /// </summary>
  /// <param name="permissionId">The ID of the permission to add.</param>
  public void AddPermission(Guid permissionId)
  {
    if (_permissions.Any(p => p.PermissionId == permissionId))
    {
      throw new InvalidOperationException("Permission already assigned to the role.");
    }

    var permission = new RolePermissionChildEntity(Id, permissionId);
    _permissions.Add(permission);

    AddDomainEvent(new RolePermissionAddedEvent(Id, permissionId));
  }

  /// <summary>
  ///   Removes a permission from the role.
  /// </summary>
  /// <param name="permissionId">The ID of the permission to remove.</param>
  public void RemovePermission(Guid permissionId)
  {
    var permission = _permissions.SingleOrDefault(p => p.PermissionId == permissionId);
    if (permission == null)
    {
      throw new InvalidOperationException("Permission not found in the role.");
    }

    _permissions.Remove(permission);

    AddDomainEvent(new RolePermissionRemovedEvent(Id, permissionId));
  }

  /// <summary>
  ///   Updates the name and title of the role.
  /// </summary>
  /// <param name="name">New name for the role.</param>
  /// <param name="title">New title for the role.</param>
  public void UpdateRole(string name, string title)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Role name cannot be empty.", nameof(name));
    }

    if (string.IsNullOrWhiteSpace(title))
    {
      throw new ArgumentException("Role title cannot be empty.", nameof(title));
    }

    Name = name;
    Title = title;

    AddDomainEvent(new RoleUpdatedEvent(Id, Name, Title));
  }
}
