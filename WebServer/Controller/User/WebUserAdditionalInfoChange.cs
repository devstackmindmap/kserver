using AkaDB.MySql;
using AkaEnum;
using Common.Entities.User;
using CommonProtocol;
using System.Threading.Tasks;
using WebLogic.User;

namespace WebServer.Controller.User
{
    public class WebUserAdditionalInfoChange : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserInfoChange;
            ResultType resultType = ResultType.Success;

            if (req.UserInfoType == UserAdditionalInfoType.NicknameChange)
            {
                var slangFilter = new AkaUtility.SlangFilter();
                if (slangFilter.IsFiltered(req.UserValue))
                    return new ProtoResult { ResultType = ResultType.Slang };
            }
            
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    await accountDb.BeginTransactionCallback(async () =>
                    {
                        await userDb.BeginTransactionCallback(async () =>
                        {
                            var userInfoChanger = UserAdditionalInfoFactory.CreateUserInfoChanger(accountDb, userDb, req.UserId, req.UserInfoType);
                            resultType = await userInfoChanger.Change(new RequestValue { StringValue = req.UserValue });

                            return true;
                        });
                        return true;
                    });
                }
            }

            return new ProtoResult { ResultType = resultType };
        }
    }
}
