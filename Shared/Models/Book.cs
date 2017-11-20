using System;

namespace Shared.Catalog
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
        public SectionType Section { get; set; }

        #endregion

        #region .ctor

        public Book() { }

        public Book(string title, string authors, uint publicationYear, SectionType section)
        {
            Title = title;
            Authors = authors;
            PublicationYear = publicationYear;
            Section = section;
        }

        #endregion
    }
}
