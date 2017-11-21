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

        [OperationContract(IsOneWay = true)]
        void SaveBooks(List<Book> books);
    }
}
