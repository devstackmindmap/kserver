using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SquareObjectSimulator.Controllers
{
    using AkaEnum;
    using Services;
    using VO;

    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        // GET: api/UserInfo
        [HttpGet]
        public IEnumerable<UserVO> Get()
        {
            return UserInfo.Instance.ToList().Select(userdao => userdao.VO);
        }

        // GET: api/UserInfo/5
        [HttpGet("{id}", Name = "Get")]
        public UserVO Get(string id)
        {
            return UserInfo.Instance.Get(id).VO;
        }

        [HttpGet("add/{id}")]
        public UserVO Add(string id)
        {
            UserInfo.Instance.Add(id);
            return Get(id);            
        }

        [HttpGet("del/{id}")]
        public bool Remove(string id)
        {
            return UserInfo.Instance.Delete(id);
        }


        [HttpGet("runmode")]
        public IEnumerable<string> Runmode()
        {
            return UserInfo.Instance.RunModes.Select(runmode=>runmode.ToString());
        }


        [HttpGet("select/{id}")]
        public string SelectRunmode(string id, [FromQuery] string runmodeString)
        {
            try
            {
                UserInfo.Instance.LoadDatas(id, runmodeString);

            }
            catch(Exception e)
            {
                return "Error";
            }

            return runmodeString;
        }


        [HttpGet("getmode/{id}")]
        public string GetRunmode(string id)
        {
            var user = Get(id);
            return UserInfo.Instance.Get(id).SelectedRunMode.ToString();
            //   return UserInfo.Instance.RunModes.FirstOrDefault(runmode => runmode.ToString().ToLower() == user.run.ToLower()).ToString();
        }







        // POST: api/UserInfo
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/UserInfo/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
