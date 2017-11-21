using Shared.Catalog;
using System;
using System.Globalization;
using System.Windows.Data;
using loc = Shared.Localization.Properties.Resources;

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
                    return loc.Section_Administration;
                case SectionType.Analytics:
                    return loc.Section_Analytics;
                case SectionType.Programming:
                    return loc.Section_Programming;
                case SectionType.ProjectManagement:
                    return loc.Section_ProjectManagement;
                case SectionType.Testing:
                    return loc.Section_Testing;
                default:
                    return sectionType.ToString();
            }
        }
    }
}
