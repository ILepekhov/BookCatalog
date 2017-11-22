using Shared.Catalog;
using System.Collections.Generic;

namespace Shared.Interfaces
{
    /// <summary>
    /// Интерфейс источника книг
    /// </summary>
    public interface IBooksSource
    {
        /// <summary>
        /// Получить список книг
        /// </summary>
        /// <returns></returns>
        List<Book> GetBooks();
        /// <summary>
        /// Сохранить список книг
        /// </summary>
        bool SaveBooks(List<Book> books);
    }
}
