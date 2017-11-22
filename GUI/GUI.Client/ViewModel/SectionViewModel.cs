using Shared.Binding;
using Shared.Catalog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        /// <summary>
        /// Заполнить секцию книгами (в GUI-потоке)
        /// </summary>
        public void AddBooksRange(IEnumerable<Book> books)
        {
            var sortedBooks = books.OrderBy(b => b.Title);

            App.Current.Dispatcher.Invoke(() =>
            {
                foreach (var book in sortedBooks)
                {
                    Books.Add(book);
                }
            });
        }

        #endregion
    }
}
