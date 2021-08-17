using AkaEnum;
using System.Threading.Tasks;

namespace Common.Entities.User
{
    public interface IUserInfoChanger
    {
        Task<ResultType> Change(RequestValue requestValue);
    }
}
