using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public sealed class Book
    {
        #region Properties

        public string Authors { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public uint PublicationYear { get; set; }
        public byte[] Image { get; set; }
        public string EBookURL { get; set; }
        public CatalogSection Section { get; set; }

        #endregion

        #region .ctor

        public Book(string title, string authors, uint publicationYear, CatalogSection section)
        {
            Title = title;
            Authors = authors;
            PublicationYear = publicationYear;
            Section = section;
        }

        #endregion
    }
}
