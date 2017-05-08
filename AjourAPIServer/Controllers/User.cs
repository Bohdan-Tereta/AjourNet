using AjourAPIServer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace AjourAPIServer.Controllers
{

    [RoutePrefix("api/Users")]
    public class UserController: ApiController
    {
        private readonly Random randomInstance = new Random();
        private const string _chars = "abcdefghijklmnopqrstuvwxyz";

        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            var json = new JavaScriptSerializer().Serialize(GetRandomUserId());
            return Ok(json);
        }

        private List<UserInfo> GetRandomUserId()
        {
            List<UserInfo> info = new List<UserInfo>();
            
            int rand = randomInstance.Next(10, 30);

            for (int i = 0; i < rand; i++)
            {
                UserInfo userToGenerate = new UserInfo { Id = RandomString(4) };
                info.Add(userToGenerate);
            }

            return info;
        }

        private string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = _chars[randomInstance.Next(_chars.Length)];
            }

            return new string(buffer);
        }
    }
}