using GymManagementSystem.Presentation.WebApi;

// Create a modular builder for the Clean Architecture Web API module.
// The modular builder initializes the application with the specified module as the entry point.
// Note: This setup uses Autofac as the dependency injection (DI) container.
var builder =
  BonyanApplication.CreateModularBuilder<GymManagementSystemWebApiModule>("GymManagementSystem", args: args);

// Add services to the container.
// Additional services and configurations can be registered here if needed.
// For example, you can configure Swagger/OpenAPI documentation by following the guide at https://aka.ms/aspnetcore/swashbuckle.

// Build the application.
// This assembles all registered modules, services, and configurations into a ready-to-run application instance.
var app = await builder.BuildAsync();

// Run the application.
// This starts the web server and listens for incoming HTTP requests.
await app.RunAsync();
