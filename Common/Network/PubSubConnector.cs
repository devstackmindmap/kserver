namespace Network
{
    public class PubSubConnector : CommonNetworkConnector
    {
        PubSubConnector(string name) : base(name)
        {

        }

        private static PubSubConnector _instance = null;
        public static PubSubConnector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PubSubConnector("Pub Sub");
                }
                return _instance;
            }
        }

    }
}
