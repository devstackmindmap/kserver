using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Mail
{
    public interface IMail
    {
        Task<ProtoMailInfo> GetMailInfo(string languageType);

        Task<ProtoMailReadAllResult> MailReadAll(List<uint> mailIds);
        Task<ProtoMailActionResult> MailRead(uint mailId, bool setRead);
        Task<ProtoMailDeleteAllResult> MailDeleteAll(List<uint> mailIds);
    }
}
