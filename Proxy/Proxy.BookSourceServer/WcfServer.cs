using Interfaces;
using Proxy.BookSourceServer.Engine.DataManagement;
using Shared.Catalog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Xml;
using loc = Shared.Localization.Properties.Resources;

namespace Proxy.BookSourceServer.Engine
{
    [ServiceBehavior(MaxItemsInObjectGraph = int.MaxValue, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class WcfServer : IWcfServer
    {
        #region Const

        private const string AddressIdentifier = "BookCatalog";

        #endregion

        #region Fields

        private string _publicationAddress;
        private uint _publicationPort;

        private readonly BookCatalogContext _catalogContext;

        private ServiceHost _serviceHost;

        #endregion

        #region Events

        public event EventHandler<string> EventOccured;



        #endregion

        #region Properties

        public bool Started { get; private set; }

        #endregion

        #region .ctor

        public WcfServer()
        {
            _catalogContext = new BookCatalogContext();
        }

        #endregion

        #region Methods

        #region IWcfServer

        public List<Book> GetBooks()
        {
            OnEventOccured(loc.RequestRorListOfBooks);

            try
            {
                _catalogContext.Books.Load();

                return _catalogContext.Books.Local.ToList();
            }
            catch (Exception e)
            {
                OnEventOccured(e.Message);
            }

            return new List<Book>();
        }

        public void SaveBooks(List<Book> books)
        {
            OnEventOccured(loc.RequestToSaveBookList);

            try
            {
                _catalogContext.Database.ExecuteSqlCommand("delete from [Books]");

                foreach (var book in books)
                {
                    _catalogContext.Books.Add(book);
                }

                _catalogContext.SaveChanges();
            }
            catch (Exception e)
            {
                OnEventOccured(e.Message);
            }
        }

        #endregion

        public void StartServer(string address, uint port)
        {
            if (Started) return;

            _publicationAddress = address;
            _publicationPort = port;

            var uriString = $"net.tcp://{_publicationAddress}:{_publicationPort}/{AddressIdentifier}";

            try
            {
                _serviceHost = new ServiceHost(this, new Uri(uriString));

                _serviceHost.AddServiceEndpoint(typeof(IWcfServer), GetConfiguringBinding(), uriString);
                _serviceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                _serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());

                new ServiceMetadataBehavior().HttpGetEnabled = true;

                _serviceHost.Open();

                Started = true;

                OnEventOccured(loc.ServiceStarted);
            }
            catch (Exception e)
            {
                OnEventOccured(e.Message);
            }
        }

        public void StopServer()
        {
            if (!Started || _serviceHost == null) return;

            try
            {
                _serviceHost.Close();

                Started = false;

                OnEventOccured(loc.ServiceStopped);
            }
            catch (Exception e)
            {
                OnEventOccured(e.Message);
            }
        }

        #endregion

        #region Helpers

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

        private void OnEventOccured(string errorText)
        {
            EventOccured?.Invoke(this, errorText);
        }

        #endregion
    }
}
