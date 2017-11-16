using Shared.BookLib;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Shared.BindingConverters
{
    [ValueConversion(typeof(CatalogSection), typeof(string))]
    public sealed class CatalogSectionToStringConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((CatalogSection)value)
            {
                case CatalogSection.Programming:
                    return "Программирование";
                case CatalogSection.Analytics:
                    return "Аналитика";
                case CatalogSection.ProjectManagement:
                    return "Управление проектами";
                case CatalogSection.Testing:
                    return "Тестирование";
                case CatalogSection.Administration:
                    return "Администрирование";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
