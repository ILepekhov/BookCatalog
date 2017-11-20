using Shared.Catalog;
using Shared.Interfaces;
using System.Collections.Generic;

namespace FileBooksSource
{
    public sealed class XmlFileBooksSource : IBooksSource
    {
        #region Fields

        private readonly string _booksSourceFilePath;

        #endregion

        #region .ctor

        public XmlFileBooksSource(string filePath)
        {
            _booksSourceFilePath = filePath;
        }

        #endregion

        #region Methods

        public List<Book> GetBooks()
        {
            return BookCatalogXmlSerializerHelper.Deserialize(_booksSourceFilePath);
        }

        public void SaveBooks(List<Book> books)
        {
            BookCatalogXmlSerializerHelper.Serialize(books, _booksSourceFilePath);
        }

        #endregion
    }
}
