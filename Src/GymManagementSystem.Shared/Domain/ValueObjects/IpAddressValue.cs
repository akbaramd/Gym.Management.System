namespace GymManagementSystem.Shared.Domain.ValueObjects;

public class IpAddressValue : BonValueObject
{
  public IpAddressValue(string value)
  {
    if (string.IsNullOrWhiteSpace(value) || !IPAddress.TryParse(value, out _))
    {
      throw new ArgumentException("Invalid IP address.", nameof(value));
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
