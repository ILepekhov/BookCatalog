using Shared.Catalog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FileBooksSource
{
    public static class BookCatalogXmlSerializerHelper
    {
        #region Methods

        public static List<Book> Deserialize(string filePath)
        {
            if (!File.Exists(filePath))
                throw new IOException($"BookCatalogXmlSerializerHelper -> Deserialize: file \"{filePath}\" not found");

            List<Book> books = null;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));

                books = (List<Book>)serializer.Deserialize(fs);
            }

            return books;
        }

        public static void Serialize(List<Book> books, string filePath)
        {
            if (books == null || books.Count == 0)
                throw new ArgumentNullException("BookCatalogXmlSerializerHelper -> Serialize: books in null or empty");

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));

                serializer.Serialize(fs, books);
            }
        }

        #endregion
    }
}
