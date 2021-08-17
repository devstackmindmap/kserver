using AkaDB.MySql;
using AkaEnum;
using Common.Entities.PayRewardedCheck;
using CommonProtocol;
using NUnit.Framework;
using System.Threading.Tasks;
using WebLogic.Store;
using WebServer.Controller.Store;

namespace WebLogicTest
{
    public class PayRewardedTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        //[Test]
        //public async Task SetPayRewardedTest()
        //{
        //    using (var accountDb = new DBContext("AccountDBSetting"))
        //    {
        //        var payRewarded = new PayRewarded(accountDb, 1);
        //        await payRewarded.SetStoreInfo(new ProtoStoreInfo
        //        {
        //            PurchaseToken = "1",
        //            StoreProductId = "2",
        //            TransactionId = "3"
        //        }, PlatformType.Google, 1);
        //    }

        //    using (var accountDb = new DBContext("AccountDBSetting"))
        //    {
        //        var payRewarded = new PayRewarded(accountDb, 1);
        //        await payRewarded.SetStoreInfo(new ProtoStoreInfo
        //        {
        //            PurchaseToken = "4",
        //            StoreProductId = "5",
        //            TransactionId = "6"
        //        }, PlatformType.Apple, 1);
        //    }

        //    using (var accountDb = new DBContext("AccountDBSetting"))
        //    {
        //        var payRewarded = new PayRewarded(accountDb, 1);
        //        await payRewarded.SetStoreInfo(new ProtoStoreInfo
        //        {
        //            PurchaseToken = "7",
        //            StoreProductId = "8",
        //            TransactionId = "9"
        //        }, PlatformType.Apple, 0);
        //    }
        //}

        //[Test]
        //public async Task UnSetPayRewardedTest()
        //{
        //    using (var accountDb = new DBContext("AccountDBSetting"))
        //    {
        //        var payRewarded = new PayRewarded(accountDb, 1);
        //        await payRewarded.SetStoreInfo(new ProtoStoreInfo
        //        {
        //            PurchaseToken = "7",
        //            StoreProductId = "8",
        //            TransactionId = "9"
        //        }, PlatformType.Apple, 0);
        //    }
        //}

        [Test]
        public async Task GetPayRewardedTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var payRewarded = new PayRewarded(accountDb, 1);
                var storeinfos = await payRewarded.GetPendingStoreInfos();
            }
        }
    }
}
