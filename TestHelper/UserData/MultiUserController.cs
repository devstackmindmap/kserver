using System.Linq;
using System.Threading.Tasks;

namespace TestHelper
{
    public class MultiUserController
    {
        private UserController[] userControllers;
        public void MakeUserList(int count /* , Rank */)
        {
            userControllers = Enumerable.Range(0, count).Select(n => new UserController()).ToArray();
            userControllers.AsParallel().ForAll(usercontrol => usercontrol.MakeOneUserData().Wait());

            userControllers.Zip(userControllers, (userController, connector) => (user: userController, connector: connector));
        }
    }
}
