using System.Globalization;
using Bonyan.Layer.Domain.DomainService;
using GymManagementSystem.Domain.IdentityContext.PermissionAggregate.Repositories;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate.Repositories;

namespace GymManagementSystem.Domain.IdentityContext.DomainService;

/// <summary>
/// Domain service for handling role-related operations.
/// </summary>
public class RoleDomainService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RoleDomainService(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
    }

    /// <summary>
    /// Creates a new role with validation and normalization.
    /// </summary>
    public async Task<BonDomainResult<RoleEntity>> CreateRoleAsync(string name, string title)
    {
        name = NormalizeText(name);
        title = NormalizeText(title);

        if (await _roleRepository.FindByNameAsync(name) != null)
            return BonDomainResult<RoleEntity>.Failure("A role with this name already exists.");

        var role = new RoleEntity(name, title);
        await _roleRepository.AddAsync(role,true);

        return BonDomainResult<RoleEntity>.Success(role);
    }

    /// <summary>
    /// Updates an existing role with validation and normalization.
    /// </summary>
    public async Task<BonDomainResult<RoleEntity>> UpdateRoleAsync(Guid roleId, string name, string title)
    {
        name = NormalizeText(name);
        title = NormalizeText(title);

        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return BonDomainResult<RoleEntity>.Failure("Role not found.");

        if (await _roleRepository.FindByNameAsync(name) != null && role.Name != name)
            return BonDomainResult<RoleEntity>.Failure("A role with this name already exists.");

        role.UpdateRole(name, title);
        await _roleRepository.UpdateAsync(role,true);

        return BonDomainResult<RoleEntity>.Success(role);
    }

    /// <summary>
    /// Adds a permission to a role with validation.
    /// </summary>
    public async Task<BonDomainResult<RoleEntity>> AddPermissionToRoleAsync(Guid roleId, Guid permissionId)
{
    var role = await _roleRepository.GetByIdAsync(roleId);
    if (role == null)
        return BonDomainResult<RoleEntity>.Failure("Role not found.");

    var permission = await _permissionRepository.GetByIdAsync(permissionId);
    if (permission == null)
        return BonDomainResult<RoleEntity>.Failure("Permission not found.");

    try
    {
        role.AddPermission(permissionId);

        // Ensure this call includes 'true'
        await _roleRepository.UpdateAsync(role, true);

        return BonDomainResult<RoleEntity>.Success(role);
    }
    catch (InvalidOperationException ex)
    {
        return BonDomainResult<RoleEntity>.Failure(ex.Message);
    }
}


    /// <summary>
    /// Removes a permission from a role with validation.
    /// </summary>
    public async Task<BonDomainResult<RoleEntity>> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return BonDomainResult<RoleEntity>.Failure("Role not found.");

        try
        {
            role.RemovePermission(permissionId);
            await _roleRepository.UpdateAsync(role,true);
            return BonDomainResult<RoleEntity>.Success(role);
        }
        catch (InvalidOperationException ex)
        {
            return BonDomainResult<RoleEntity>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Updates the permissions of a role to match the given input permissions.
    /// </summary>
    public async Task<BonDomainResult<RoleEntity>> UpdatePermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return BonDomainResult<RoleEntity>.Failure("Role not found.");

        // Fetch all permissions
        var allPermissions = await _permissionRepository.FindAsync(x=>true);
        var validPermissionIds = new HashSet<Guid>(allPermissions.Select(p => p.Id));

        // Validate input permissions
        foreach (var permissionId in permissionIds)
        {
            if (!validPermissionIds.Contains(permissionId))
                return BonDomainResult<RoleEntity>.Failure($"Permission with ID {permissionId} does not exist.");
        }

        // Determine permissions to add and remove
        var currentPermissionIds = role.Permissions.Select(p => p.PermissionId).ToHashSet();
        var permissionsToAdd = permissionIds.Except(currentPermissionIds);
        var permissionsToRemove = currentPermissionIds.Except(permissionIds);

        // Update role permissions
        foreach (var permissionId in permissionsToRemove)
        {
            role.RemovePermission(permissionId);
        }

        foreach (var permissionId in permissionsToAdd)
        {
            role.AddPermission(permissionId);
        }

        await _roleRepository.UpdateAsync(role,true);
        return BonDomainResult<RoleEntity>.Success(role);
    }

    /// <summary>
    /// Deletes a role by ID.
    /// </summary>
    public async Task<BonDomainResult<bool>> DeleteRoleAsync(Guid roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return BonDomainResult<bool>.Failure("Role not found.");

        await _roleRepository.DeleteAsync(role,true);
        return BonDomainResult<bool>.Success(true);
    }

    /// <summary>
    /// Normalizes text by trimming whitespace and capitalizing the first letter of each word.
    /// </summary>
    private string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        text = text.Trim();
        var cultureInfo = new CultureInfo("en-US"); // Replace with your desired culture
        return cultureInfo.TextInfo.ToTitleCase(text.ToLower(cultureInfo));
    }
}
