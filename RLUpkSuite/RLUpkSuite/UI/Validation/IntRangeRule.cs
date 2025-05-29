using System.Globalization;
using System.Windows.Controls;

namespace RlUpk.RLUpkSuite.UI.Validation;

public class IntRangeRule : ValidationRule
{
    private static readonly ValidationResult ValueRequired = new(false, "Value required");

    private static readonly ValidationResult NotANumberResult = new(false, "Not a number");

    public int Min { get; set; }

    public int Max { get; set; }

    public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
    {
        if (value is not string stringValue)
        {
            return ValueRequired;
        }

        if (string.IsNullOrWhiteSpace(stringValue))
        {
            return ValueRequired;
        }

        if (!Int32.TryParse(stringValue, out int intValue))
        {
            return NotANumberResult;
        }

        if (intValue < Min || intValue > Max)
        {
            return new ValidationResult(false, $"Please enter an value in the range: {Min}-{Max}.");
        }

        return ValidationResult.ValidResult;
    }
}