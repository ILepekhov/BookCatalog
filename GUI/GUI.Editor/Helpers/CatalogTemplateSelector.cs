using Shared.Catalog;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Editor.Helpers
{
    public sealed class CatalogTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BookTemplate { get; set; }
        public DataTemplate SectionTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Book book)
            {
                return BookTemplate;
            }
            return SectionTemplate;
        }
    }
}
