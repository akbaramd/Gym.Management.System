using Bonyan.Modularity.Abstractions;

namespace GymManagementSystem.Domain.Tests;

/// <summary>
///   Test module for the Clean Architecture domain layer.
///   This module includes dependencies from the domain module for testing purposes.
/// </summary>
public class GymManagementSystemDomainTestModule : BonModule
{
  public GymManagementSystemDomainTestModule()
  {
    // Include domain module as a dependency for testing
    DependOn<GymManagementSystemDomainModule>();
  }
}
