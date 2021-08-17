using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Product;
using CommonProtocol;
using NUnit.Framework;
using System.Threading.Tasks;
using WebLogic.Store;
using WebServer.Controller.Store;

namespace WebLogicTest
{
    public class StoreTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task GetProductBuyCondition()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var buy = ProductBuyFactory.CreateProductBuy(accountDb, null, 1, 30002, AkaEnum.ProductTableType.EventDigital, PlatformType.Google);
                //var condition = buy.SetProductBuyCondition(accountDb);
            }
        }

        [Test]
        public async Task GetProducts()
        {
            ProtoOnGetProducts protoProducts = new ProtoOnGetProducts();
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(1))
                {
                    ProductManager productManager = new ProductManager(accountDb, userDb, 1);
                    await productManager.GetStoreProducts(PlatformType.Google, "KR",  protoProducts);
                }
            }
        }

        [TestCase(MessageType.BuyProductDigital, (uint)1, ProductTableType.FixDigital, (uint)1)]
        [TestCase(MessageType.BuyProductDigital, (uint)5, ProductTableType.FixDigital, (uint)1)]
        [TestCase(MessageType.BuyProductDigital, (uint)6, ProductTableType.FixDigital, (uint)1)]
        [TestCase(MessageType.BuyProductDigital, (uint)7, ProductTableType.FixDigital, (uint)1)]
        [TestCase(MessageType.BuyProductDigital, (uint)9, ProductTableType.FixDigital, (uint)43)]
        [TestCase(MessageType.BuyProductDigital, (uint)10, ProductTableType.FixDigital, (uint)42)]
        [TestCase(MessageType.BuyProductDigital, (uint)11, ProductTableType.FixDigital, (uint)42)]
        [TestCase(MessageType.BuyProductDigital, (uint)13, ProductTableType.FixDigital, (uint)1)]
        [TestCase(MessageType.BuyProductDigital, (uint)14, ProductTableType.EventDigital, (uint)1)]
        [TestCase(MessageType.BuyProductDigital, (uint)90000, ProductTableType.UserDigital, (uint)1)]
        [TestCase(MessageType.BuyProductDigital, (uint)90000, ProductTableType.UserDigital, (uint)2)]
        public async Task ProductBuyTest(MessageType messageType, uint productId, ProductTableType productTableType, uint userId)
        {
            var web = new WebProductBuyDigital();

            var req = new ProtoBuyProductDigital
            {
                MessageType = messageType,
                ProductId = productId,
                ProductTableType = productTableType,
                UserId = userId
            };
            var res = await web.DoPipeline(req);
        }

        [Test]
        public async Task PaidGemTest()
        {
            using (var db = new AkaDB.MySql.DBContext(1))
            {
                var material = Common.Entities.Item.MaterialFactory.CreateMaterial(AkaEnum.MaterialType.Gem, 1, 10, db);
                await material.Get("Test");

                var material2 = Common.Entities.Item.MaterialFactory.CreateMaterial(AkaEnum.MaterialType.GemPaid, 1, 20, db);
                await material2.Get("Test");
            }
        }

        [Test]
        public async Task PaidGemIsEnoughCountTest()
        {
            using (var db = new AkaDB.MySql.DBContext(1))
            {
                var material = Common.Entities.Item.MaterialFactory.CreateMaterial(AkaEnum.MaterialType.Gem, 1, 31, db);
                if (await material.IsEnoughCount())
                    Assert.True(true);
                else
                    Assert.True(false);
            }
        }

        [Test]
        public async Task PaidGemIsEnoughCountTest2()
        {
            using (var db = new AkaDB.MySql.DBContext(1))
            {
                var material = Common.Entities.Item.MaterialFactory.CreateMaterial(AkaEnum.MaterialType.GemPaid, 1, 31, db);
                if (await material.IsEnoughCount())
                    Assert.True(true);
                else
                    Assert.True(false);
            }
        }

        [Test]
        public async Task PaidGemUseTest()
        {
            using (var db = new AkaDB.MySql.DBContext(1))
            {
                var material = Common.Entities.Item.MaterialFactory.CreateMaterial(AkaEnum.MaterialType.Gem, 1, 27, db);
                if (await material.IsEnoughCount())
                {
                    await material.Use("Test");
                }
                else
                    Assert.True(false);
            }
        }
    }
}
