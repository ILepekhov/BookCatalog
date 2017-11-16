
using Shared.BookLib;
using System;
using System.Collections.Generic;

namespace Shared.Catalog
{
    public sealed class Section
    {
        #region Properties

        public CatalogSection CatalogSection { get; }

        public string Caption { get; }

        public List<Book> Books { get; }

        #endregion

        #region .ctor

        public Section(CatalogSection catalogSection)
        {
            CatalogSection = catalogSection;
            Caption = GetSectionCaption(catalogSection);
            Books = new List<Book>();
        }

        public Section(CatalogSection catalogSection, IEnumerable<Book> books) : this(catalogSection)
        {
            Books = new List<Book>(books);
        }

        #endregion

        #region Helpers

        private string GetSectionCaption(CatalogSection catalogSection)
        {
            switch (catalogSection)
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

        #endregion
    }
}
