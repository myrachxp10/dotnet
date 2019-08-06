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
        DMS dms = new adapi.DMS();
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

        [HttpPost]
        [Route("v1/upload")]
        public string uploaddoc(DMS.dmsDoc doc)
        {
            return "{'response':'" + dms.UploadToDMS(doc) + "'}";
        }

        [HttpGet]
        [Route("v1/delete/{docurl}")]
        public string delete(string docurl)
        {
            string url = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(docurl));
            return "{'response':'" + dms.DeleteDOC(url)+ "'}";
            
        }

        [HttpGet]
        [Route("v1/open/{docurl}")]
        public string opendoc(string docurl)
        {
            return dms.OpenDoc(docurl);
        }

        [HttpPost]
        [Route("v1/sms/send")]
        public string sendSMS(ADService.SMS sms)
        {
            return "{'response':'" + dirser.sendSMS(sms.mobileno,sms.from,sms.txt) + "'}";
            //return dirser.GetEmployeeDataFromUserID("panchaln");
        }


        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }
    }
}