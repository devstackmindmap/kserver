using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Item
{
    public class WebSkinPutOn : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var protoSkinPutOn = requestInfo as ProtoSkinPutOn;

            var resultType = ResultType.Success;
            using (var db = new DBContext(protoSkinPutOn.UserId))
            {
                var item = ItemFactory.CreateSkin(ItemType.Skin, protoSkinPutOn.UserId, protoSkinPutOn.SkinId, db);

                if (protoSkinPutOn.SkinId == 0 ||
                    (await item.IsHave() && item.IsValidData(protoSkinPutOn.UnitId)))
                    await item.PutOn(protoSkinPutOn.UnitId);
                else
                    resultType = ResultType.Fail;
            }

            return new ProtoResult
            {
                ResultType = resultType
            };
        }
    }
}
