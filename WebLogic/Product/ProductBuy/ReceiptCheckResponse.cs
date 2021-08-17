using System.Collections.Generic;

namespace WebLogic.Store
{
    public class ReceiptCheckResponse
    {
        public List<ReceiptCheckResponseItem> receipt;
    }

    public class ReceiptCheckResponseItem
    {
        public string Result;
        public string transactionId;
        public string purchaseState;
        public string purchaseDate;
        public string expirationDate;
        public string productId;
    }
}
