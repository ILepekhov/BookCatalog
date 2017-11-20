using Shared.Binding;
using Shared.Catalog;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GUI.Client.ViewModel
{
    public sealed class SectionViewModel : BaseBinding
    {
        #region Properties

        public SectionType SectionType { get; }

        public ObservableCollection<Book> Books { get; }

        #endregion

        #region .ctor

        public SectionViewModel(SectionType sectionType)
        {
            SectionType = sectionType;
            Books = new ObservableCollection<Book>();
        }

        #endregion

        #region Methods

        public void AddBooksRange(IEnumerable<Book> books)
        {
            foreach (var book in books)
            {
                Books.Add(book);
            }
        }

        #endregion
    }
}
