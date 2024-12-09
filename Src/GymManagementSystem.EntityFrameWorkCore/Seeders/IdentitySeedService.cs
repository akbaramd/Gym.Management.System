using Bonyan.UnitOfWork;
using GymManagementSystem.Domain.IdentityContext.DomainService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.Infrastructure.Data.Seeders;

public class IdentitySeedService : BackgroundService
{
  private readonly ILogger<IdentitySeedService> _logger;
  private readonly IRoleRepository _roleRepository;
  private readonly UserDomainService _userDomainService;
  private readonly IUserRepository _userRepository;
  private readonly IBonUnitOfWorkManager _workManager;

  public IdentitySeedService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    UserDomainService userDomainService,
    ILogger<IdentitySeedService> logger, IBonUnitOfWorkManager workManager)
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _workManager = workManager;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    using var uow = _workManager.Begin();

    // Check if the "admin" role exists
    var allRoles = await _roleRepository.FindAsync(x => true);
    var adminRole = allRoles.FirstOrDefault(r => r.Name.Equals("admin", StringComparison.OrdinalIgnoreCase));

    if (adminRole == null)
    {
      // Create the "admin" role if it doesn't exist
      adminRole = new RoleEntity("admin", "مدیر سیستم");
      await _roleRepository.AddAsync(adminRole, true);
      _logger.LogInformation("Admin role created.");
    }
    else
    {
      _logger.LogInformation("Admin role already exists. Skipping creation.");
    }

    // Now check if the admin user exists
    var existingAdminUser = await _userRepository.FindByPhoneNumberAsync("admin@admin.com");
    if (existingAdminUser == null)
    {
      // Create the admin user if it doesn't exist
      var newUser =
        new UserEntity("0987654321", "2749876543", "Admin", "Administrator",
          "Admin@123456"); // Replace with a valid phone number and hashed password

      var userCreationResult = await _userDomainService.CreateAsync(newUser);
      if (userCreationResult.IsSuccess)
      {
        _logger.LogInformation("Admin user created successfully.");

        // Assign the admin role to the newly created user
        var rolesResult = await _userDomainService.AssignRolesAsync(userCreationResult.Value, "admin");
        if (rolesResult.IsSuccess)
        {
          _logger.LogInformation("Admin role assigned successfully.");
        }
        else
        {
          _logger.LogError("Failed to assign admin role.");
        }
      }
      else
      {
        _logger.LogError("Failed to create admin user: {Error}", userCreationResult.ErrorMessage);
      }
    }
    else
    {
      _logger.LogInformation("Admin user already exists. Skipping user creation.");
    }

    await uow.CompleteAsync();
  }
}
