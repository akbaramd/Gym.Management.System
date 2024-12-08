using Bonyan.Modularity.Abstractions;
using GymManagementSystem.Application;

namespace GymManagementSystem.Application.Tests;

/// <summary>
/// Test module for the Clean Architecture application layer.
/// This module includes dependencies from the application module for testing purposes.
/// </summary>
public class GymManagementSystemApplicationTestModule : BonModule
{
    public GymManagementSystemApplicationTestModule()
    {
        // Include application module as a dependency for testing
        DependOn<GymManagementSystemApplicationModule>();
    }
}