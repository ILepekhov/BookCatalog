using Shared.BookLib;
using Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Catalog
{
    public sealed class BookCatalog
    {
        #region Fields

        private IBooksSource _bookSource;

        #endregion

        #region Properties

        public List<List<Book>> Sections { get; }

        #endregion

        #region .ctor

        public BookCatalog(IBooksSource bookSource)
        {
            _bookSource = bookSource;

            Sections = new List<List<Book>>();
        }

        #endregion

        #region Methods

        public void LoadBooks()
        {
            if (_bookSource != null)
            {
                DistributeBooksToSections(_bookSource.GetBooks());
            }
        }

        public void SaveBooks()
        {
            if (_bookSource != null)
            {
                _bookSource.SaveBooks(Books);
            }
        }

        #endregion

        #region Helpers

        private void DistributeBooksToSections(List<Book> books)
        {
            if (books == null || !books.Any())
                return;

            Dictionary<CatalogSection, List<Book>> tempBuffer = new Dictionary<CatalogSection, List<Book>>();

            foreach (var book in books)
            {
                if (tempBuffer.ContainsKey(book.Section))
                {
                    tempBuffer[book.Section].Add(book);
                }
                else
                {
                    tempBuffer.Add(book.Section, new List<Book> { book });
                }
            }



        }

        #endregion
    }
}