using Shared.Binding;
using Shared.Catalog;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GUI.Client.ViewModel
{
    public sealed class BookCatalogViewModel : BaseBinding
    {
        #region Fields

        private readonly BookCatalog _bookCatalog;

        private Section _selectedSection;

        #endregion

        #region Properties

        public ObservableCollection<Section> Sections { get; }

        public Section SelectedSection
        {
            get { return _selectedSection; }
            set { SetValue(ref _selectedSection, value); }
        }

        #endregion

        #region .ctor

        public BookCatalogViewModel(BookCatalog bookCatalog)
        {
            if (bookCatalog == null)
                throw new ArgumentNullException("BookCatalogViewModel -> .ctor: bookCatalog is null");

            _bookCatalog = bookCatalog;

            Sections = new ObservableCollection<Section>(bookCatalog.Sections);
            SelectedSection = Sections.FirstOrDefault();
        }

        #endregion
    }
}
