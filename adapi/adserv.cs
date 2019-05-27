using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace adapi
{
    public class adserv : ApiController
    {
        
        // GET api/<controller>
        public DirectoryService.Person Get(string user)
        {
            DirectoryService dirser = new DirectoryService();
            return dirser.GetEmployeeDataFromUserID(user); // string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        /*
        public string Get()
        {
            return "Incorrect input passed";
        }*/
        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}