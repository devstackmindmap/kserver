namespace WebLogic.Store
{
    class ReceiptCheckRequest
    {
        public uint userId;
        public string purchaseToken;
        public string productID;
        public int platform;
        public string transaction_id;
    }
}
