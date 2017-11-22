namespace GUI.Client.ViewModel
{
    public sealed class ConnectionSettings
    {
        #region Properties

        public string Address { get; set; }

        public uint Port { get; set; }

        #endregion

        #region .ctor

        public ConnectionSettings(string address, uint port)
        {
            Address = address;
            Port = port;
        }

        #endregion
    }
}
