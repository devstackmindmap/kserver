using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquareObjectSimulator.DAO
{
    using AkaEnum;
    using VO;
    public class UserDAO
    {
        public string Id { get; set; }

        public List<int> SO { get; set; }
        
        public RunMode SelectedRunMode { get; set; }

        public UserVO VO => new UserVO { Id = this.Id, SoCount = SO.Count };
    }
}
