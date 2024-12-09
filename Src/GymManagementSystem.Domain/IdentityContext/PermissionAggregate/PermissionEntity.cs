#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace GymManagementSystem.Domain.IdentityContext.PermissionAggregate;

/// <summary>
///   Permission aggregate root for managing permissions.
/// </summary>
public class PermissionEntity : BonAggregateRoot<Guid>
{
  private PermissionEntity() { } // EF Core constructor

  /// <summary>
  ///   Constructor for creating a new permission.
  /// </summary>
  /// <param name="title">The descriptive title of the permission.</param>
  /// <param name="name">The unique name of the permission.</param>
  public PermissionEntity(string title, string name)
  {
    if (string.IsNullOrWhiteSpace(title))
    {
      throw new ArgumentException("Permission title cannot be empty.", nameof(title));
    }

    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Permission name cannot be empty.", nameof(name));
    }

    Id = Guid.NewGuid();
    Title = title;
    Name = name;

    AddDomainEvent(new PermissionCreatedEvent(Id, Title, Name));
  }

  public string Title { get; private set; }
  public string Name { get; private set; }

  /// <summary>
  ///   Updates the title of the permission.
  /// </summary>
  /// <param name="newTitle">The new title for the permission.</param>
  public void UpdateTitle(string newTitle)
  {
    if (string.IsNullOrWhiteSpace(newTitle))
    {
      throw new ArgumentException("Permission title cannot be empty.", nameof(newTitle));
    }

    Title = newTitle;

    AddDomainEvent(new PermissionUpdatedEvent(Id, Title, Name));
  }

  /// <summary>
  ///   Updates the name of the permission.
  /// </summary>
  /// <param name="newName">The new name for the permission.</param>
  public void UpdateName(string newName)
  {
    if (string.IsNullOrWhiteSpace(newName))
    {
      throw new ArgumentException("Permission name cannot be empty.", nameof(newName));
    }

    Name = newName;

    AddDomainEvent(new PermissionUpdatedEvent(Id, Title, Name));
  }

  /// <summary>
  ///   Marks the permission as deleted.
  /// </summary>
  public void MarkAsDeleted()
  {
    AddDomainEvent(new PermissionDeletedEvent(Id, Title, Name));
  }
}
