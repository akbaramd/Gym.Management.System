using GymManagementSystem.Domain.IdentityContext.DomainService;
using GymManagementSystem.Domain.IdentityContext.PermissionAggregate;
using GymManagementSystem.Domain.IdentityContext.PermissionAggregate.Repositories;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate.Repositories;
using Moq;

namespace GymManagementSystem.Domain.Tests.IdentityContext.DomainServices;

public class RoleDomainServiceTests
{
  private readonly Mock<IRoleRepository> _roleRepositoryMock;
  private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
  private readonly RoleDomainService _roleDomainService;

  public RoleDomainServiceTests()
  {
    _roleRepositoryMock = new Mock<IRoleRepository>();
    _permissionRepositoryMock = new Mock<IPermissionRepository>();
    _roleDomainService = new RoleDomainService(_roleRepositoryMock.Object, _permissionRepositoryMock.Object);
  }

  [Fact]
  public async Task CreateRoleAsync_ShouldCreateRole_WhenValidInput()
  { 
    // Arrange
    string roleName = "admin";
    string roleTitle = "Administrator";

    _roleRepositoryMock.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
      .ReturnsAsync((RoleEntity?)null);
    // Act
    var result = await _roleDomainService.CreateRoleAsync(roleName, roleTitle);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal("Admin", result.Value.Name); // Ensures normalization
    Assert.Equal("Administrator", result.Value.Title);

    _roleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<RoleEntity>(),true), Times.Once);
  }

  [Fact]
  public async Task CreateRoleAsync_ShouldFail_WhenRoleNameAlreadyExists()
  {
    // Arrange
    string roleName = "admin";
    string roleTitle = "Administrator";

    var existingRole = new RoleEntity(roleName, roleTitle);
    _roleRepositoryMock.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
      .ReturnsAsync(existingRole);

    // Act
    var result = await _roleDomainService.CreateRoleAsync(roleName, roleTitle);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal("A role with this name already exists.", result.ErrorMessage);

    _roleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<RoleEntity>(),true), Times.Never);
  }

  [Fact]
  public async Task UpdateRoleAsync_ShouldUpdateRole_WhenValidInput()
  {
    // Arrange
    Guid roleId = Guid.NewGuid();
    string newRoleName = "manager";
    string newRoleTitle = "Manager";

    var existingRole = new RoleEntity("admin", "Administrator");
    _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId))
      .ReturnsAsync(existingRole);
    _roleRepositoryMock.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
      .ReturnsAsync((RoleEntity?)null);

    // Act
    var result = await _roleDomainService.UpdateRoleAsync(roleId, newRoleName, newRoleTitle);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal("Manager", result.Value.Name);
    Assert.Equal("Manager", result.Value.Title);

    _roleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<RoleEntity>(),true), Times.Once);
  }

  [Fact]
  public async Task UpdateRoleAsync_ShouldFail_WhenRoleNotFound()
  {
    // Arrange
    Guid roleId = Guid.NewGuid();
    _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId))
      .ReturnsAsync((RoleEntity?)null);

    // Act
    var result = await _roleDomainService.UpdateRoleAsync(roleId, "newName", "newTitle");

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal("Role not found.", result.ErrorMessage);

    _roleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<RoleEntity>(),true), Times.Never);
  }

  [Fact]
  public async Task AddPermissionToRoleAsync_ShouldAddPermission_WhenValid()
  {
    // Arrange
    Guid roleId = Guid.NewGuid();
    Guid permissionId = Guid.NewGuid();

    var role = new RoleEntity("admin", "Administrator");
    var permission = new PermissionEntity("ViewReports", "View Reports");

    _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId))
      .ReturnsAsync(role);
    _permissionRepositoryMock.Setup(p => p.GetByIdAsync(permissionId))
      .ReturnsAsync(permission);
        
    _roleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<RoleEntity>(), true))
      .Returns(Task.CompletedTask); // Mock UpdateAsync
    // Act
    var result = await _roleDomainService.AddPermissionToRoleAsync(roleId, permissionId);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Contains(role.Permissions, p => p.PermissionId == permissionId);

    _roleRepositoryMock.Verify(r => r.UpdateAsync(role,true), Times.Once);
  }

  [Fact]
  public async Task AddPermissionToRoleAsync_ShouldFail_WhenPermissionNotFound()
  {
    // Arrange
    Guid roleId = Guid.NewGuid();
    Guid permissionId = Guid.NewGuid();

    var role = new RoleEntity("admin", "Administrator");

    _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId))
      .ReturnsAsync(role);
    _permissionRepositoryMock.Setup(p => p.GetByIdAsync(permissionId))
      .ReturnsAsync((PermissionEntity?)null);

    // Act
    var result = await _roleDomainService.AddPermissionToRoleAsync(roleId, permissionId);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal("Permission not found.", result.ErrorMessage);

    _roleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<RoleEntity>(),true), Times.Never);
  }

  [Fact]
  public async Task UpdatePermissionsAsync_ShouldSynchronizePermissions()
  {
    // Arrange
    Guid roleId = Guid.NewGuid();
    var permission1 = new PermissionEntity("ViewReports", "View Reports") { Id = Guid.NewGuid() };
    var permission2 = new PermissionEntity("EditUsers", "Edit Users") { Id = Guid.NewGuid() };

    var role = new RoleEntity("admin", "Administrator");
    role.AddPermission(permission1.Id); // Already has permission1

    _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId))
      .ReturnsAsync(role);
    _permissionRepositoryMock.Setup(p => p.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PermissionEntity, bool>>>()))
      .ReturnsAsync(new List<PermissionEntity> { permission1, permission2 });

    var result = await _roleDomainService.UpdatePermissionsAsync(roleId, new[] { permission2.Id });

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Contains(role.Permissions, p => p.PermissionId == permission2.Id);
    Assert.DoesNotContain(role.Permissions, p => p.PermissionId == permission1.Id);

    _roleRepositoryMock.Verify(r => r.UpdateAsync(role,true), Times.Once);
  }

  [Fact]
  public async Task DeleteRoleAsync_ShouldDeleteRole_WhenExists()
  {
    // Arrange
    Guid roleId = Guid.NewGuid();
    var role = new RoleEntity("admin", "Administrator");

    _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId))
      .ReturnsAsync(role);

    // Act
    var result = await _roleDomainService.DeleteRoleAsync(roleId);

    // Assert
    Assert.True(result.IsSuccess);
    _roleRepositoryMock.Verify(r => r.DeleteAsync(role,true), Times.Once);
  }

  [Fact]
  public async Task DeleteRoleAsync_ShouldFail_WhenRoleNotFound()
  {
    // Arrange
    Guid roleId = Guid.NewGuid();

    _roleRepositoryMock.Setup(r => r.GetByIdAsync(roleId))
      .ReturnsAsync((RoleEntity?)null);

    // Act
    var result = await _roleDomainService.DeleteRoleAsync(roleId);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal("Role not found.", result.ErrorMessage);

    _roleRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<RoleEntity>(),true), Times.Never);
  }
}