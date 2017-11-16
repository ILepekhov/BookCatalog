using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Models
{
    public sealed class BookCatalog
    {
        #region Properties

        public List<Book> Books { get; private set; }

        #endregion

        #region .ctor

        public BookCatalog()
        {
            Books = new List<Book>();
        }

        #endregion

        #region Methods

        public void LoadFromXMLFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new IOException($"BookCatalog -> LoadFromXMLFile: file \"{filePath}\" not found");
            }

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));

                Books = (List<Book>)serializer.Deserialize(fs);
            }
        }

        public void SaveToXMLFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));

                serializer.Serialize(fs, Books);
            }
        }

        #endregion
    }
}
