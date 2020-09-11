using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adapi
{
    class Program
    {
        static void Main(string[] args)
        {
            //adapi.ADService adser = new ADService();
            adapi.MailAPI.ReadMailItems();
            //mailapi = new MailAPI();
            

            //Console.WriteLine(adser.getAuthentication("appdev","Did10n@c"));
            //Console.WriteLine(adser.SendMail("noreply@kecdigital.com", "panchaln@kecrpg.com","panchaln@kecrpg.com","","Hi", "Hello", "Welcome", "Nilesh"));
            
        }
    }
}
