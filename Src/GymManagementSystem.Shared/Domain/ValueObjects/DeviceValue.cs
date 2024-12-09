namespace GymManagementSystem.Shared.Domain.ValueObjects;

public class DeviceValue : BonValueObject
{
  public DeviceValue(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("Device information cannot be empty.", nameof(value));
    }

    Value = value;
  }

  public string Value { get; }

  public override string ToString()
  {
    return Value;
  }

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    return [Value];
  }
}
