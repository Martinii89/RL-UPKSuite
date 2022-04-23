using System.Globalization;
using Decryptor.Wpf.MVVM.Model;

namespace Decryptor.Wpf.Converters;

/// <summary>
/// Converts <see cref="UnpackResult"/> to a string representation
/// </summary>
public class UnpackResultValueConverter : ConverterMarkupExtension<UnpackResultValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var enumType = value.GetType();
        var convert = Enum.GetName(enumType, value);
        return convert ?? string.Empty;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}