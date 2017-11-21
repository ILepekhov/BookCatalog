using System;
using System.Runtime.Serialization;

namespace Shared.Catalog
{
    [Serializable]
    [DataContract]
    public sealed class Book
    {
        #region Properties

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Authors { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int PublicationYear { get; set; }
        [DataMember]
        public byte[] Image { get; set; }
        [DataMember]
        public string EBookURL { get; set; }
        [DataMember]
        public SectionType Section { get; set; }

        #endregion

        #region .ctor

        public Book() { }

        public Book(string title, string authors, int publicationYear, SectionType section)
        {
            Title = title;
            Authors = authors;
            PublicationYear = publicationYear;
            Section = section;
        }

        #endregion

        #region Methods

        public void CopyFrom(Book book)
        {
            Authors = book.Authors;
            Title = book.Title;
            Description = book.Description;
            PublicationYear = book.PublicationYear;
            Image = book.Image;
            EBookURL = book.EBookURL;
            Section = book.Section;
        }

        #endregion
    }
}
