using Shared.Catalog;
using System.Data.Entity;

namespace Proxy.BookSourceServer.Engine.DataManagement
{
    public sealed class BookCatalogContext : DbContext
    {
        #region Properties

        public DbSet<Book> Books { get; set; }

        #endregion
        #region .ctor

        public BookCatalogContext() : base("SQLiteConnection") { }

        #endregion

    }
}
