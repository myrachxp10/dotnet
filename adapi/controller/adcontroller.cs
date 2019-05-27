using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace adapi.controller
{
    public class adcontroller : ApiController
    {
        // GET api/<controller>
        ADService dirser = new adapi.ADService();
        // GET api/<controller>
        [HttpGet]
        [Route("v1/user/{user}")]
        public ADService.Person Get(string user)
        {          
            return dirser.GetEmployeeDataFromUserID(user);
        }

        [HttpGet]
        [Route("v1/auth/{cred}")]
        public bool authUser(string cred)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(cred);
            string creds = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            int seperatorIndex = creds.IndexOf(':');
            string user = creds.Substring(0, seperatorIndex);
            string pwd = creds.Substring(seperatorIndex + 1);
            return dirser.getAuthentication(user,pwd); 
        }

        [HttpPost]
        [Route("v1/mail")]
        public string send(ADService.Mail mail)
        {
            return "{'response':'" + dirser.SendMail(mail) +"'}";
            //return dirser.GetEmployeeDataFromUserID("panchaln");
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }
    }
}