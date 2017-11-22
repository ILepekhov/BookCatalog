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

        public bool SaveBooks(List<Book> books)
        {
            try
            {
                BookCatalogXmlSerializerHelper.Serialize(books, _booksSourceFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
