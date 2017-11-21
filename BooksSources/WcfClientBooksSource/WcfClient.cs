using Interfaces;
using Shared.Catalog;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace WcfClientBooksSource
{
    public sealed class WcfClient : IBooksSource
    {
        #region Const

        private const string AddressIdentifier = "BookCatalog";

        #endregion

        #region Fields

        private string _serverAddress;
        private uint _serverPort;

        #endregion

        #region .ctor

        public WcfClient(string serverAddress, uint serverPort)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
        }

        #endregion

        #region Methods

        public List<Book> GetBooks()
        {
            var books = new List<Book>();

            try
            {
                var factory = GetChannelFactory();
                var serverChannel = factory.CreateChannel();
                books = serverChannel.GetBooks();

                CloseChannelFactory(factory);
            }
            catch (Exception e)
            {
                // toDo: сделать обработку ошибок
            }

            return books;
        }

        public void SaveBooks(List<Book> books)
        {
            try
            {
                var factory = GetChannelFactory();
                var serverChannel = factory.CreateChannel();
                serverChannel.SaveBooks(books);

                CloseChannelFactory(factory);
            }
            catch (Exception e)
            {
                // toDo: сделать обработку ошибок
            }
        }

        #endregion

        #region Helpers

        private string GetServerUriString()
        {
            return $"net.tcp://{_serverAddress}:{_serverPort}/{AddressIdentifier}";
        }

        private NetTcpBinding GetConfiguringBinding()
        {
            var binding = new NetTcpBinding();

            binding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            binding.Security.Mode = SecurityMode.None;
            binding.ReceiveTimeout = TimeSpan.FromSeconds(10);
            binding.SendTimeout = TimeSpan.FromSeconds(10);
            binding.MaxConnections = 10;
            binding.ListenBacklog = 10;

            binding.MaxBufferPoolSize = 1024 * 1024 * 1024;
            binding.MaxBufferSize = 1024 * 1024 * 1024;
            binding.MaxReceivedMessageSize = 1024 * 1024 * 1024;

            binding.ReaderQuotas = XmlDictionaryReaderQuotas.Max;

            return binding;
        }

        private ChannelFactory<IWcfServer> GetChannelFactory()
        {
            return new ChannelFactory<IWcfServer>(GetConfiguringBinding(), GetServerUriString());
        }

        private void CloseChannelFactory(IChannelFactory channelFactory)
        {
            if (channelFactory == null) return;

            channelFactory.Close();
            channelFactory.Abort();
        }

        #endregion
    }
}
