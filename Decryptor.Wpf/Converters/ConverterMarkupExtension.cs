using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Decryptor.Wpf.Converters;

public abstract class ConverterMarkupExtension<T> : MarkupExtension, IValueConverter where T: class, new()
{
    private static readonly T Converter = new();

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Converter;
    }
    
    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
}