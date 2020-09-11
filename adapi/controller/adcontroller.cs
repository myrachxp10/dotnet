using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
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
            string source = ConfigurationManager.AppSettings["source"].ToString();
            if (source == "0")
            {
                //return dirser.GetEmployeeDataFromUserID(user);
                return dirser.GetADUser(user);
            }
            else { return dirser.GetPersonFromUserLogin(user);
            }
            //return dirser.GetEmpDataFromUserID(user);
        }

        [HttpGet]
        [Route("v1/unlock/{acnt}")]
        public ADService.Response unlockme(string acnt) {
            return dirser.unlockme(acnt);
        }

        [HttpGet]
        [Route("v1/getmail/{guid}")]
        public ADService.Response GetEmailFromGUID(string guid)
        {
            return dirser.GetEmailFromGUID(guid.ToUpper());
        }

        [HttpGet]
        [Route("v1/getdacc/{empcode}")]
        public ADService.Response GetDomainIDFromEmpCode(string empcode)
        {            
            return dirser.GetDomainAccountFromEmpCode(empcode);
        }

        /*
        [HttpGet]
        [Route("v1/syncdacc/{param}")]
        public ADService.Response[] SyncDomainAccount(string param)
        {
            return dirser.SyncDomainAccount(param);
        }*/
        

        [HttpGet]
        [Route("v1/acc/{user}")]
        public ADService.AccountInfo GetAccountInfo(string user)
        {
            //return dirser.GetADUser(user);
            return dirser.getAccountDetails(user);
        }
        
        [HttpGet]
        [Route("v1/auth/{cred}")]
        public bool authUser(string cred)
        {
            
            string user = string.Empty;
            string pwd = string.Empty;
            var base64EncodedBytes = System.Convert.FromBase64String(cred);
            string creds = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            int seperatorIndex = creds.IndexOf(':');

            user = creds.Substring(0, seperatorIndex);
            pwd = creds.Substring(seperatorIndex + 1);
            
            if(user=="" || pwd== ""){
                var allUrlKeyValues = ControllerContext.Request.GetQueryNameValuePairs();
                if (user == "")
                    user = allUrlKeyValues.LastOrDefault(x => x.Key == "username").Value;
                if(pwd=="")
                    pwd = allUrlKeyValues.LastOrDefault(x => x.Key == "password").Value;
                if (user == null) user = "";
                if (pwd == null) pwd = "";

            }
            return dirser.getAuthentication(user, pwd);
        }

        /*
        [HttpGet]
        [Route("v1/auth/{cred}")]
        public bool authUser(string cred)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(cred);
            string creds = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            int seperatorIndex = creds.IndexOf(':');
            //var queryString = actionContext.Request.GetQueryNameValuePairs().ToLookup(x => x.Key, x => x.Value);
            string user = creds.Substring(0, seperatorIndex);
            string pwd = creds.Substring(seperatorIndex + 1);
            return dirser.getAuthentication(user, pwd);
        }
        */

        [HttpGet]
        [Route("v1/authtoken/{cred}")]
        public ADService.Response authUserToken(string cred)
        {
            return dirser.getAuth(cred);
        }

        [HttpPost]
        [Route("v1/update")]
        public ADService.Response updateADUser(ADService.ADUpdate obj)
        {
            //myObjectReference.SetAdInfo("sAMAccountName" , Property.Title, "Vice President", "company.org")
            return dirser.SetAdInfo(obj.objectFilter,obj.objectName,obj.objectValue);
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