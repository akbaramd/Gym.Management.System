using Bonyan.Layer.Domain.DomainService;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate;
using GymManagementSystem.Domain.IdentityContext.RoleAggregate.Repositories;
using GymManagementSystem.Domain.IdentityContext.UserAggregate;
using GymManagementSystem.Domain.IdentityContext.UserAggregate.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GymManagementSystem.Domain.IdentityContext.DomainService;

/// <summary>
/// Domain service for user-related operations.
/// </summary>
public class UserDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserDomainService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    /// <summary>
    /// Creates a new user with normalization and validation.
    /// </summary>
    public async Task<BonDomainResult<UserEntity>> CreateAsync(UserEntity user)
    {
        NormalizeUser(user);

        // validation
        if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
            return BonDomainResult<UserEntity>.Failure("First name and last name cannot be empty.");
        if (!ValidatePhoneNumber(user.PhoneNumber))
            return BonDomainResult<UserEntity>.Failure("Invalid phone number format.");
        if (!IsValidLanguage(user.FirstName) || !IsValidLanguage(user.LastName))
            return BonDomainResult<UserEntity>.Failure("Names must adhere to language rules.");
        
        // logic 

        if (await _userRepository.FindByPhoneNumberAsync(user.PhoneNumber) != null)
          return BonDomainResult<UserEntity>.Failure("A user with this phone number already exists.");
        await _userRepository.AddAsync(user, true);

        // result
        return BonDomainResult<UserEntity>.Success(user);
    }

    /// <summary>
    /// Updates an existing user with normalization, validation, and cascading rules.
    /// </summary>
    public async Task<BonDomainResult<UserEntity>> UpdateAsync(UserEntity user)
    {
        NormalizeUser(user);

        var existingUser = await _userRepository.GetByIdAsync(user.Id);
        if (existingUser == null)
            return BonDomainResult<UserEntity>.Failure("User not found.");

        if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
            return BonDomainResult<UserEntity>.Failure("First name and last name cannot be empty.");

        if (!IsValidLanguage(user.FirstName) || !IsValidLanguage(user.LastName))
            return BonDomainResult<UserEntity>.Failure("Names must adhere to language rules.");

        existingUser.UpdateProfile(user.FirstName, user.LastName);
        await _userRepository.UpdateAsync(existingUser, true);

        return BonDomainResult<UserEntity>.Success(existingUser);
    }

    
 /// <summary>
    /// Assigns multiple roles to a user, ensuring no duplicates.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="roleNames">The names of the roles to assign.</param>
    public async Task<BonDomainResult<UserEntity>> AssignRolesAsync(Guid userId, params string[] roleNames)
    {
        if (roleNames.Length == 0)
            return BonDomainResult<UserEntity>.Failure("Role names cannot be null or empty.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return BonDomainResult<UserEntity>.Failure("User not found.");

        // Normalize role names
        var normalizedRoleNames = roleNames.Select(NormalizeText).Distinct().ToArray();

        // Fetch all roles
        var allRoles = (await _roleRepository.FindAsync(x => true)).ToList();
        var roleDict = allRoles.ToDictionary(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase);

        // Assign roles
        foreach (var roleName in normalizedRoleNames)
        {
            if (!roleDict.TryGetValue(roleName, out var roleId))
                return BonDomainResult<UserEntity>.Failure($"Role '{roleName}' does not exist.");

            if (!user.Roles.Any(r => r.RoleId == roleId))
                user.AssignRole(roleId);
        }

        // Update user
        await _userRepository.UpdateAsync(user, true);
        return BonDomainResult<UserEntity>.Success(user);
    }

    /// <summary>
    /// Updates a user's roles to match the given input roles.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="roleNames">The desired roles for the user.</param>
    public async Task<BonDomainResult<UserEntity>> UpdateRolesAsync(Guid userId, params string[] roleNames)
    {
        if ( roleNames.Length == 0)
            return BonDomainResult<UserEntity>.Failure("Role names cannot be null or empty.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return BonDomainResult<UserEntity>.Failure("User not found.");

        // Normalize role names
        var normalizedRoleNames = roleNames.Select(NormalizeText).Distinct().ToArray();

        // Fetch all roles
        var allRoles = (await _roleRepository.FindAsync(x => true)).ToList();
        var roleDict = allRoles.ToDictionary(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase);

        // Ensure all input roles exist
        foreach (var roleName in normalizedRoleNames)
        {
            if (!roleDict.ContainsKey(roleName))
                return BonDomainResult<UserEntity>.Failure($"Role '{roleName}' does not exist.");
        }

        // Determine roles to unassign and assign
        var currentRoleNames = user.Roles
            .Select(userRole => allRoles.First(r => r.Id == userRole.RoleId).Name)
            .Select(NormalizeText)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var rolesToAssign = normalizedRoleNames.Except(currentRoleNames, StringComparer.OrdinalIgnoreCase);
        var rolesToUnassign = currentRoleNames.Except(normalizedRoleNames, StringComparer.OrdinalIgnoreCase);

        // Unassign roles
        foreach (var roleName in rolesToUnassign)
        {
            var roleId = roleDict[roleName];
            user.UnassignRole(roleId);
        }

        // Assign roles
        foreach (var roleName in rolesToAssign)
        {
            var roleId = roleDict[roleName];
            user.AssignRole(roleId);
        }

        // Update user
        await _userRepository.UpdateAsync(user, true);
        return BonDomainResult<UserEntity>.Success(user);
    }

    /// <summary>
    /// Unassigns multiple roles from a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="roleNames">The names of the roles to unassign.</param>
    public async Task<BonDomainResult<UserEntity>> UnassignRolesAsync(Guid userId, params string[] roleNames)
    {
      if ( roleNames.Length == 0)
        return BonDomainResult<UserEntity>.Failure("Role names cannot be null or empty.");

      var user = await _userRepository.GetByIdAsync(userId);
      if (user == null)
        return BonDomainResult<UserEntity>.Failure("User not found.");

      // Normalize role names
      var normalizedRoleNames = roleNames.Select(NormalizeText).Distinct().ToArray();

      // Fetch all roles
      var allRoles = (await _roleRepository.FindAsync(x => true)).ToList();
      var roleDict = allRoles.ToDictionary(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase);

      // Unassign roles
      foreach (var roleName in normalizedRoleNames)
      {
        if (!roleDict.TryGetValue(roleName, out var roleId))
          return BonDomainResult<UserEntity>.Failure($"Role '{roleName}' does not exist.");

        if (user.Roles.Any(r => r.RoleId == roleId))
          user.UnassignRole(roleId);
      }

      // Update user
      await _userRepository.UpdateAsync(user, true);
      return BonDomainResult<UserEntity>.Success(user);
    }

    
    /// <summary>
    /// Validates a phone number format.
    /// </summary>
    private bool ValidatePhoneNumber(string phoneNumber)
    {
        // Normalize the phone number
        phoneNumber = NormalizePhoneNumber(phoneNumber);

        // Validation logic: must be numeric and match the required length
        return Regex.IsMatch(phoneNumber, @"^\d{10,15}$");
    }

    /// <summary>
    /// Normalizes a phone number by removing non-numeric characters.
    /// </summary>
    private string NormalizePhoneNumber(string phoneNumber)
    {
        return Regex.Replace(phoneNumber, @"[^\d]", string.Empty);
    }

    /// <summary>
    /// Normalizes a user entity's properties.
    /// </summary>
    private void NormalizeUser(UserEntity user)
    {
      user.UpdateProfile(NormalizeText(user.FirstName),NormalizeText(user.LastName));
      user.UpdateProfile(NormalizeText(user.FirstName),NormalizeText(user.LastName));
    }

    /// <summary>
    /// Normalizes text by trimming whitespace and capitalizing the first letter of each word.
    /// </summary>
    private string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        text = text.Trim();
        var cultureInfo = new CultureInfo("en-US"); // Replace with your target culture if needed
        return cultureInfo.TextInfo.ToTitleCase(text.ToLower(cultureInfo));
    }

    /// <summary>
    /// Checks if a string adheres to specific language rules.
    /// </summary>
    private bool IsValidLanguage(string input)
    {
        // Example: restrict to letters, spaces, and basic punctuation
        return Regex.IsMatch(input, @"^[\p{L}\p{M}\s'.-]+$");
    }

    

    /// <summary>
    /// Deletes a user by their ID with cascading rules.
    /// </summary>
    public async Task<BonDomainResult<bool>> DeleteAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return BonDomainResult<bool>.Failure("User not found.");

        if (user.Roles.Any())
            return BonDomainResult<bool>.Failure("Cannot delete a user with assigned roles.");

        await _userRepository.DeleteAsync(user, true);
        return BonDomainResult<bool>.Success(true);
    }
}
