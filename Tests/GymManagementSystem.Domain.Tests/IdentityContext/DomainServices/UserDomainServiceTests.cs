using GymManagementSystem.Domain.IdentityContext.DomainService;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate.Repositories;
using GymManagementSystem.Domain.IdentityContext.UserAggregate;
using GymManagementSystem.Domain.IdentityContext.UserAggregate.Repositories;
using GymManagementSystem.Shared.Domain.ValueObjects;
using Moq;

namespace GymManagementSystem.Domain.Tests.IdentityContext.DomainServices;

public class UserDomainServiceTests
{
  private readonly Mock<IUserRepository> _userRepositoryMock;
  private readonly Mock<IRoleRepository> _roleRepositoryMock;
  private readonly UserDomainService _userDomainService;
  private static readonly MediaVo DefaultMediaVo = new MediaVo("default.jpg", "default.jpg", ".jpeg", 1024);

  public UserDomainServiceTests()
  {
    _userRepositoryMock = new Mock<IUserRepository>();
    _roleRepositoryMock = new Mock<IRoleRepository>();
    _userDomainService = new UserDomainService(_userRepositoryMock.Object, _roleRepositoryMock.Object);
  }

  private UserEntity CreateDefaultUser() =>
    new UserEntity("1234567890", "123456789", "John", "Doe","Aa@123456");

  private List<RoleEntity> CreateDefaultRoles() =>
    new List<RoleEntity>
    {
      new RoleEntity("Admin", "Administrator"),
      new RoleEntity("Editor", "Content Editor")
    };

  // --- CREATE ASYNC ---
  [Fact]
  public async Task CreateAsync_ShouldCreateUser_WhenValidInput()
  {
    var user = CreateDefaultUser();

    _userRepositoryMock.Setup(r => r.FindByPhoneNumberAsync(user.PhoneNumber))
      .ReturnsAsync((UserEntity?)null);

    _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<bool>()))
      .Returns((UserEntity entity, bool autoSave) => Task.FromResult(entity));

    var result = await _userDomainService.CreateAsync(user);

    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal(user.PhoneNumber, result.Value.PhoneNumber);
    _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), true), Times.Once);
  }

  [Fact]
  public async Task CreateAsync_ShouldFail_WhenPhoneNumberIsInvalid()
  {
    var user = CreateDefaultUser();
    user.UpdatePhoneNumber("InvalidPhone");

    var result = await _userDomainService.CreateAsync(user);

    Assert.False(result.IsSuccess);
    Assert.Equal("Invalid phone number format.", result.ErrorMessage);
    _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), true), Times.Never);
  }

  [Fact]
  public async Task CreateAsync_ShouldFail_WhenUserAlreadyExists()
  {
    var user = CreateDefaultUser();

    _userRepositoryMock.Setup(r => r.FindByPhoneNumberAsync(user.PhoneNumber))
      .ReturnsAsync(user);

    var result = await _userDomainService.CreateAsync(user);

    Assert.False(result.IsSuccess);
    Assert.Equal("A user with this phone number already exists.", result.ErrorMessage);
    _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), true), Times.Never);
  }

  // --- UPDATE ASYNC ---
  [Fact]
  public async Task UpdateAsync_ShouldUpdateUser_WhenValidInput()
  {
    var user = CreateDefaultUser();
    var existingUser = CreateDefaultUser();
    existingUser.UpdateProfile("Jane", "Smith");

    _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
      .ReturnsAsync(existingUser);

    _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<bool>()))
      .Returns(Task.CompletedTask);

    var result = await _userDomainService.UpdateAsync(user);

    Assert.True(result.IsSuccess);
    Assert.Equal(user.FirstName, existingUser.FirstName);
    Assert.Equal(user.LastName, existingUser.LastName);
    _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), true), Times.Once);
  }

  [Fact]
  public async Task UpdateAsync_ShouldFail_WhenUserNotFound()
  {
    var user = CreateDefaultUser();

    _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
      .ReturnsAsync((UserEntity?)null);

    var result = await _userDomainService.UpdateAsync(user);

    Assert.False(result.IsSuccess);
    Assert.Equal("User not found.", result.ErrorMessage);
    _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), true), Times.Never);
  }

  // --- ASSIGN ROLES ---
  [Fact]
  public async Task AssignRolesAsync_ShouldAssignRoles_WhenValidInput()
  {
    var user = CreateDefaultUser();
    var roles = CreateDefaultRoles();

    _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
      .ReturnsAsync(user);

    _roleRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RoleEntity, bool>>>()))
      .ReturnsAsync(roles);

    _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<bool>()))
      .Returns(Task.CompletedTask);

    var result = await _userDomainService.AssignRolesAsync(user, "Admin", "Editor");

    Assert.True(result.IsSuccess);
    Assert.Contains(user.Roles, r => r.RoleId == roles[0].Id);
    Assert.Contains(user.Roles, r => r.RoleId == roles[1].Id);
    _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), true), Times.Once);
  }

  [Fact]
  public async Task AssignRolesAsync_ShouldFail_WhenRoleDoesNotExist()
  {
    var user = CreateDefaultUser();
    var roles = CreateDefaultRoles();

    _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
      .ReturnsAsync(user);

    _roleRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RoleEntity, bool>>>()))
      .ReturnsAsync(roles);

    var result = await _userDomainService.AssignRolesAsync(user, "NonExistentRole");

    Assert.False(result.IsSuccess);
    _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), true), Times.Never);
  }

  // --- DELETE ASYNC ---
  [Fact]
  public async Task DeleteAsync_ShouldDeleteUser_WhenValid()
  {
    var user = CreateDefaultUser();

    _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
      .ReturnsAsync(user);

    _userRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<UserEntity>(), It.IsAny<bool>()))
      .Returns(Task.CompletedTask);

    var result = await _userDomainService.DeleteAsync(user.Id);

    Assert.True(result.IsSuccess);
    _userRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<UserEntity>(), true), Times.Once);
  }

  [Fact]
  public async Task DeleteAsync_ShouldFail_WhenUserHasRoles()
  {
    var user = CreateDefaultUser();
    user.AssignRole(Guid.NewGuid());

    _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
      .ReturnsAsync(user);

    var result = await _userDomainService.DeleteAsync(user.Id);

    Assert.False(result.IsSuccess);
    Assert.Equal("Cannot delete a user with assigned roles.", result.ErrorMessage);
    _userRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<UserEntity>(), true), Times.Never);
  }

  [Fact]
  public async Task DeleteAsync_ShouldFail_WhenUserNotFound()
  {
    var userId = Guid.NewGuid();

    _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
      .ReturnsAsync((UserEntity?)null);

    var result = await _userDomainService.DeleteAsync(userId);

    Assert.False(result.IsSuccess);
    Assert.Equal("User not found.", result.ErrorMessage);
    _userRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<UserEntity>(), true), Times.Never);
  }


}