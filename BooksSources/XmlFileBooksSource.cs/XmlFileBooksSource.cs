using Shared.BookLib;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FileBooksSource
{
    public sealed class XmlFileBooksSource : IBooksSource
    {
        #region Consts

        private const string BooksSourceFileName = "BooksSource.xml";

        #endregion

        #region Fields

        private readonly string _booksSourceFilePath;

        #endregion

        #region .ctor

        public XmlFileBooksSource()
        {
            _booksSourceFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}{BooksSourceFileName}";
        }

        #endregion

        #region Methods

        public List<Book> GetBooks()
        {
            var books = new List<Book>();

            if (!File.Exists(_booksSourceFilePath))
            {
                SaveBooks(books);

                return books;
            }

            using (var fs = new FileStream(_booksSourceFilePath, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));

                books = (List<Book>)serializer.Deserialize(fs);
            }

            return books;
        }

        public void SaveBooks(List<Book> books)
        {
            using (var fs = new FileStream(_booksSourceFilePath, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));

                serializer.Serialize(fs, books);
            }
        }

        #endregion
    }
}
