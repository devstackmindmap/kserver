using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonProtocol;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SquareObjectSimulator.Controllers
{
    using Services;
    using SquareObjectSimulator.VO;

    [ApiController]
    [Route("sop")]
    public class SquareObjectController : Controller
    {
        private readonly ILogger<SquareObjectController> _logger;

        public SquareObjectController(ILogger<SquareObjectController> logger)
        {
            _logger = logger;
        }


        // GET: sop/5/list
        [HttpGet("{id}/list")]
        public IEnumerable<SquareObjectVO> Get(string id)
        {
            return UserInfo.Instance.Get(id).SO.Select(soId => SquareObject.Instance.Get(soId));
        }



        // GET: sop/5
        [HttpGet("{id}/so")]
        public SquareObjectVO GetSO(int id)
        {
            var so = SquareObject.Instance.GetObject(id);
            return so;
        }



        [HttpGet("{id}/new")]
        public SquareObjectVO New(string id)
        {
            var soid = SquareObject.Instance.New();

            UserInfo.Instance.Get(id)?.SO.Add(soid);

            return GetSO(soid);
        }

        [HttpGet("{id}/del")]
        public void Remove(int id)
        {
            foreach (var user in UserInfo.Instance.Users)
            {
                if (user.Value.SO.Remove(id))
                {                    
                    SquareObject.Instance.Delete(id);
                    break;
                }
            }
        }

        [HttpGet("{id}/start/{level}")]
        public SquareObjectVO Start(int id, int level)
        {
            return SquareObject.Instance.Start(id,level);
        }

        [HttpGet("{id}/restart")]
        public SquareObjectVO Restart(int id)
        {
            return SquareObject.Instance.Restart(id);
        }


        [HttpGet("{id}/reward")]
        public SquareObjectVO Reward(int id)
        {
            return SquareObject.Instance.GetReward(id);
        }

        [HttpGet("{id}/stop")]
        public SquareObjectVO Stop(int id)
        {
            return SquareObject.Instance.Stop(id);
        }

        [HttpPost("update")]
        public SquareObjectVO UpdateObject(SquareObjectVO vo)
        {
            return SquareObject.Instance.UpdateObject(vo);
        }


        [HttpGet("{id}/levelup/{type}")]
        public SquareObjectVO Levelup(int id, int type)
        {
            return SquareObject.Instance.Levelup(id, type,false);
        }

        [HttpGet("{id}/leveldown/{type}")]
        public SquareObjectVO Leveldown(int id, int type)
        {
            return SquareObject.Instance.Leveldown(id, type,false);
        }

        [HttpGet("{id}/forcelevelup/{type}")]
        public SquareObjectVO ForceLevelup(int id, int type)
        {
            return SquareObject.Instance.Levelup(id, type, true);
        }

        [HttpGet("{id}/forceleveldown/{type}")]
        public SquareObjectVO ForceLeveldown(int id, int type)
        {
            return SquareObject.Instance.Leveldown(id, type, true);
        }





        [HttpGet("{id}/power/{energy}")]
        public SquareObjectVO PowerInjection(int id, int energy)
        {
            return SquareObject.Instance.EnergyInjection(id, energy,0);
        }

        [HttpGet("{id}/box/{energy}")]
        public SquareObjectVO BoxInjection(int id, int energy)
        {
            return SquareObject.Instance.EnergyInjection(id, 0, energy);
        }

        [HttpGet("{id}/donate")]
        public SquareObjectVO Donation(int id)
        {
            return SquareObject.Instance.Donation(id);
        }

        [HttpGet("{id}/help")]
        public SquareObjectVO HelpMe(int id)
        {
            return SquareObject.Instance.HelpMe(id);
        }
    }
}
