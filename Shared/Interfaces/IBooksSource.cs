using Shared.BookLib;
using System.Collections.Generic;

namespace Shared.Interfaces
{
    public interface IBooksSource
    {
        List<Book> GetBooks();
        void SaveBooks(List<Book> books);
    }
}
