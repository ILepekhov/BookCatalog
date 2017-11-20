using Shared.Catalog;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Shared.Binding
{
    [ValueConversion(typeof(SectionType), typeof(string))]
    public class CatalogTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetSectionCaption((SectionType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetSectionCaption(SectionType sectionType)
        {
            switch (sectionType)
            {
                case SectionType.Administration:
                    return "Администрирование";
                case SectionType.Analytics:
                    return "Аналитика";
                case SectionType.Programming:
                    return "Программирование";
                case SectionType.ProjectManagement:
                    return "Управление проектами";
                case SectionType.Testing:
                    return "Тестирование";
                default:
                    return sectionType.ToString();
            }
        }
    }
}
