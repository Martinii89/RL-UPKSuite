using System;
using System.Globalization;
using System.Windows.Data;

namespace Decryptor.Wpf.Converters;

public class FileDropEventArgsConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var dropArgs = value as DragEventArgs;
        if (dropArgs == null)
        {
            throw new ArgumentException("Expected value to be of type DragEventArgs");
        }

        if (!dropArgs.Data.GetDataPresent(DataFormats.FileDrop))
        {
            throw new ArgumentException("Expected value data te be of type DataFormat.FileDrop");
        }

        string[] files = (string[])dropArgs.Data.GetData(DataFormats.FileDrop);
        return files;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}