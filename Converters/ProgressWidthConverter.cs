using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using System.Windows.Data;


namespace Library.Converters
{
    public class ProgressWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values == null || values.Length < 2)
                    return 0;

                // Проверка типа и null для первого параметра
                if (!(values[0] is double totalWidth && !double.IsNaN(totalWidth) && totalWidth > 0))
                    return 0;

                // Проверка типа и null для второго параметра
                if (!(values[1] is double value && !double.IsNaN(value)))
                    return 0;

                // Здесь предположим, что Value идет от 0 до Maximum (обычно 100)
                double percentage = value / 100.0;

                return totalWidth * percentage;
            }
            catch
            {
                return 0; // Если вдруг вообще что-то пошло не так, возвращаем безопасный ноль
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
