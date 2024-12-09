namespace GymManagementSystem.Presentation.WebApi.OperationFilter;

public class SessionHeaderOperationFilter : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    // Add Device header
    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "Device",
      In = ParameterLocation.Header,
      Description = "Device details (e.g., browser or app information)",
      Required = false,
      Schema = new OpenApiSchema { Type = "string" }
    });

    // Add IP Address header
    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "IP-Address",
      In = ParameterLocation.Header,
      Description = "IP Address of the client",
      Required = false,
      Schema = new OpenApiSchema { Type = "string" }
    });
  }
}