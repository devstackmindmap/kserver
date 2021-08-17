using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.InfusionBox;
using CommonProtocol;

namespace WebServer.Controller.Box
{
    public class WebInfusionBoxOpen : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var protoInfusionBoxOpen = requestInfo as ProtoInfusionBoxOpen;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var db = new DBContext(protoInfusionBoxOpen.UserId))
                {
                    var infusionBoxOpen
                        = await InfusionBoxOpenFactory.CreateInfusionBoxOpen(protoInfusionBoxOpen.UserId, protoInfusionBoxOpen.Type, db, accountDb);

                    if (!await infusionBoxOpen.IsEnoughEnergy())
                        return new ProtoOnInfusionBoxOpen { ResultType = ResultType.NeedEnergy };

                    var openInfo = await infusionBoxOpen.GetInfusionBoxOpenInfo();

                    await db.BeginTransactionCallback(async () =>
                    {
                        await infusionBoxOpen.SetInfusionBoxOpenInfo(openInfo);
                        return true;
                    });

                    AkaLogger.Log.User.InfusionBox.Log(protoInfusionBoxOpen.UserId,
                                                        (byte)protoInfusionBoxOpen.Type,
                                                        string.Join(",", openInfo.ItemResults.Select(item => $"{item.ClassId}:{item.Count}")),
                                                        openInfo.NewInfusionBox.Id,
                                                        openInfo.NewInfusionBox.NewTotalBoxEnergy,
                                                        openInfo.NewInfusionBox.NewTotalUserBonusEnergy,
                                                        openInfo.NewInfusionBox.NewTotalUserEnergy);

                    return new ProtoOnInfusionBoxOpen
                    {
                        ResultType = ResultType.Success,
                        OpenInfo = new ProtoInfusionBoxOpenInfo
                        {
                            ItemResults = openInfo.ItemResults,
                            NewInfusionBox = openInfo.NewInfusionBox
                        }
                    };
                }
            }
        }
    }
}
