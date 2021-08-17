using AkaEnum;
using NUnit.Framework;
using CommonProtocol;
using System.Threading.Tasks;
using WebServer.Controller.User;

namespace WebLogicTest
{
    class WebSetEmoticonTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task Run()
        {
            var web = new WebServer.Controller.Emoticon.WebSetEmoticon();

            var emoticonInfoList = new System.Collections.Generic.List<ProtoEmoticonInfo>();
            emoticonInfoList.Add(new ProtoEmoticonInfo
            {
                EmoticonId = 1,
                OrderNum = 0,
                UnitId = 1001
            });
            var result = await web.DoPipeline(new ProtoSetEmoticons
            {
                MessageType = MessageType.SetEmoticons,
                UserId = 42,
                Emoticons = emoticonInfoList
            });
            var onProto = result as ProtoResult;
        }
    }
}
