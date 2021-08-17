using Common;
using CommonProtocol;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestHelper;
using WebServer.Controller.SquareObject;

namespace WebLogicTest
{
    public class WebSquareObjectTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }
        
        [TestCase((uint)6390, (uint)1, TestName = "SquareObject Start Test", Category = Category.SquareObject)]
        public async Task SquareObjectStartTest(uint userId, uint objectLevel)
        {
            var webServerUri = $"http://127.0.0.1:{AkaConfig.Config.GameServerConfig.GameServerPort}/";

            WebServerRequestor webServer = new WebServerRequestor();
            var result = webServer.Request(MessageType.SquareObjectStart, AkaSerializer.AkaSerializer<ProtoSquareObjectStart>.Serialize(new ProtoSquareObjectStart
            {
                MessageType = MessageType.SquareObjectStart,
                UserId = userId,
                ObjectLevel = objectLevel
            }), webServerUri);

            var protoResult = AkaSerializer.AkaSerializer<ProtoOnSquareObjectStart>.Deserialize(result);
            Console.WriteLine(protoResult.Result);

            Assert.That(protoResult.Result == AkaEnum.SquareObjectResponseType.Success);

        }


        [TestCase((uint)6390,  TestName = "SquareObject Get info Test", Category = Category.SquareObject)]
        public async Task SquareObjectGetInfoTest(uint userId)
        {
            var webServerUri = $"http://127.0.0.1:{AkaConfig.Config.GameServerConfig.GameServerPort}/";
            WebServerRequestor webServer = new WebServerRequestor();
            var result = webServer.Request(MessageType.GetSquareObjectState, AkaSerializer.AkaSerializer<ProtoUserId>.Serialize(new ProtoUserId
            {
                MessageType = MessageType.GetSquareObjectState,
                UserId = userId
            }), webServerUri);

            var protoResult = AkaSerializer.AkaSerializer<ProtoOnGetSquareObject>.Deserialize(result);
            Console.WriteLine(protoResult.Result +" --"+ protoResult.SquareObjectInfo.CurrentState.InvasionHistory.Count);

            Assert.That(protoResult.Result == AkaEnum.SquareObjectResponseType.Success);

        }


        [TestCase((uint)6390, TestName = "StartSquareObject  Test", Category = Category.SquareObject)]
        public async Task StartSquareObjectTest(uint userId)
        {
            var api = new WebSquareObjectStart();
            var result = (ProtoOnSquareObjectStart)await api.DoPipeline(new ProtoSquareObjectStart
            {
                MessageType = MessageType.GetSquareObjectState,
                UserId = userId,
                ObjectLevel = 1
            });

            Console.WriteLine(result.Result.ToString() + " - " + result.SquareObjectInfo.CurrentState.InvasionHistory.Count);


        }


        [TestCase((uint)6390, TestName = "SquareObject Get Test", Category = Category.SquareObject)]
        public async Task GetSquareObjectTest(uint userId)
        {
            var api = new WebGetSquareObject();
            var result = (ProtoOnGetSquareObject)await api.DoPipeline(new ProtoUserId
            {
                MessageType = MessageType.GetSquareObjectState,
                UserId = userId
            });

            Console.WriteLine(result.Result.ToString() + " - " + result.SquareObjectInfo.CurrentState.InvasionHistory.Count);
        }

        [TestCase((uint)6390, TestName = "SquareObject MultipleGet Test", Category = Category.SquareObject)]
        public async Task GetSquareObjectMultiple(uint userId)
        {
            var caller = Enumerable.Range(0, 5);
            var tasks = caller.Select(_ =>
           {
               var api = new WebGetSquareObject();
               return api.DoPipeline(new ProtoUserId
               {
                   MessageType = MessageType.GetSquareObjectState,
                   UserId = userId
               });
           });
            var results = await Task.WhenAll(tasks);

            foreach(var result in results.Cast<ProtoOnGetSquareObject>())
                Console.WriteLine(result.Result.ToString() + " - " + result.SquareObjectInfo.CurrentState.InvasionHistory.Count);

        }


        [TestCase((uint)3, 100, 200, TestName = "SquareObject Power Injection Test", Category = Category.SquareObject)]
        public async Task SquareObjectInjectionTest(uint userId, int spower, int apower)
        {
            var webServerUri = $"http://127.0.0.1:{AkaConfig.Config.GameServerConfig.GameServerPort}/";

            WebServerRequestor webServer = new WebServerRequestor();
            var result = webServer.Request(MessageType.SquareObjectPowerInjection, AkaSerializer.AkaSerializer<ProtoSquareObjectPowerInject>.Serialize(new ProtoSquareObjectPowerInject
            {
                MessageType = MessageType.SquareObjectPowerInjection,
                UserId = userId,
                SquareObjectEnergy = spower,
                AgencyEnergy = apower
            }), webServerUri);

            var protoResult = AkaSerializer.AkaSerializer<ProtoOnSquareObjectPowerInject>.Deserialize(result);
            Console.WriteLine(protoResult.Result);

            Assert.That(protoResult.Result == AkaEnum.SquareObjectResponseType.Success);

        }


        [TestCase(3u, TestName = "스퀘어오브젝트 친구목록")]
        public async Task GetFriendsList(uint userId)
        {

            var api = new WebGetSquareObjectFriends();
            var protoResult = (ProtoOnGetSquareObjectFriends)await api.DoPipeline(new ProtoUserId
            {
                MessageType = MessageType.GetSquareObjectFriends,
                UserId = userId
            });



            Console.WriteLine($"{userId} Help Friends\n\t <{string.Join(",", protoResult.NeedHelpFriends.Select(user=>user.UserId))}>");
            Console.WriteLine($"{userId} Done Friends\n\t <{string.Join(",", protoResult.DonatedFriends.Select(user => user.UserId))}>");


          //  Assert.That(protoResult.Result == AkaEnum.SquareObjectResponseType.Success);

        }


        [TestCase(3u, 6390u, TestName = "스퀘어오브젝트 친구 에너지주입")]
        public async Task GetFriendInjection(uint userId, uint targetId)
        {

            var api = new WebSquareObjectPowerInjectFriend();
            var protoResult = (ProtoOnSquareObjectPowerInjectFriend)await api.DoPipeline(new ProtoUserIdTargetId
            {
                MessageType = MessageType.SquareObjectPowerInjectionFriend,
                UserId = userId,
                TargetId = targetId
            });



            Console.WriteLine($"result = {protoResult.Result.ToString()}");


            //  Assert.That(protoResult.Result == AkaEnum.SquareObjectResponseType.Success);

        }


    }
}
