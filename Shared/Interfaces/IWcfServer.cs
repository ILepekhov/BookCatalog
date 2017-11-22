using Shared.Catalog;
using System.Collections.Generic;
using System.ServiceModel;

namespace Interfaces
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IWcfServer
    {
        [OperationContract]
        List<Book> GetBooks();

        [OperationContract]
        bool SaveBooks(List<Book> books);
    }
}
