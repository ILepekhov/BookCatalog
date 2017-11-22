using Shared.Binding;
using Shared.Catalog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace GUI.Editor.ViewModel
{
    public sealed class SectionViewModel : BaseBinding
    {
        #region Properties

        public SectionType SectionType { get; }

        public ObservableCollection<Book> Books { get; }

        #endregion

        #region Commands

        public ICommand AddBookCmd { get; }

        public ICommand RemoveBookCmd { get; }

        #endregion

        #region .ctor

        public SectionViewModel(SectionType sectionType)
        {
            SectionType = sectionType;
            Books = new ObservableCollection<Book>();

            AddBookCmd = new DelegateCommand(ExecAddBookCmd, _ => true);
            RemoveBookCmd = new DelegateCommand(ExecRemoveBookCmd, _ => true);
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

        #region AddBookCmd

        private void ExecAddBookCmd(object parameter)
        {
            Books.Add(new Book()
            {
                Title = "Новая книга",
                Section = SectionType,
            });
        }

        #endregion

        #region RemoveBookCmd

        private void ExecRemoveBookCmd(object parameter)
        {
            if (parameter is Book b)
            {
                Books.Remove(b);
            }
        }

        #endregion
    }
}
