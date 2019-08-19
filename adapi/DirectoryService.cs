using System;
using System.IO;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Collections;
using System.Net.Mail;
using System.Net;

namespace adapi
{
    public class ADService
    {

        private static string sPath = ConfigurationManager.AppSettings["ldap"].ToString();
        private static string _uid = ConfigurationManager.AppSettings["_uid"].ToString();
        private static string _pwd = ConfigurationManager.AppSettings["_pwd"].ToString();

        private DirectoryEntry myDirectory = new DirectoryEntry(sPath, _uid, _pwd); // pass the user account and password for your Enterprise admin.
        
        public string GetUserInfo(string inSAM, string inType, string attr)
        {
            // Public Function GetUserInfo(ByVal inSAM As String, ByVal inType As String) As String
            try
            {
                string SamAccount = inSAM; // Strings.Right(inSAM, Strings.Len(inSAM) - Strings.InStr(inSAM, @"\"));
                DirectorySearcher mySearcher = new DirectorySearcher(myDirectory);
                SearchResultCollection mySearchResultColl;
                SearchResult mySearchResult;
                ResultPropertyCollection myResultPropColl;
                ResultPropertyValueCollection myResultPropValueColl;
                mySearcher.Filter = ("(&(objectClass=user)(" + attr + "=" + SamAccount + "))");
                mySearchResultColl = mySearcher.FindAll();

                switch (mySearchResultColl.Count)
                {
                    case 0:
                        {
                            return null;
                            //return;
                        }

                    case object _ when mySearchResultColl.Count > 1:
                        {
                            return null;
                            //return;
                        }
                }
                mySearchResult = mySearchResultColl[0];
                myResultPropColl = mySearchResult.Properties;
                myResultPropValueColl = myResultPropColl[inType];
                if (myResultPropValueColl.Count > 0)
                    return System.Convert.ToString(myResultPropValueColl[0]);
                else
                    return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public string GetFullNamefromUserID(string UserAccount)
        {
            string _fname, _ini, _lname;
            _fname = GetUserInfo(UserAccount, "givenName", "samaccountname");
            _ini = ""; // GetUserInfo(UserAccount, "initials", "samaccountname")
            _lname = GetUserInfo(UserAccount, "sn", "samaccountname");
            if (_ini.Trim() == "0")
                _ini = "";
            return _fname + " " + _ini + " " + _lname;
        }

        public string GetFirstNamefromUserID(string UserAccount)
        {
            string _fname;
            _fname = GetUserInfo(UserAccount, "givenName", "samaccountname");
            return _fname;
        }

        public string GetEmpCodeFromUserID(string UserAccount)
        {
            string _empcode;
            _empcode = GetUserInfo(UserAccount, "employeeid", "samaccountname");
            if (_empcode == null) { _empcode = "900013566"; }
            return _empcode;
        }

        public string GetLastNamefromUserID(string UserAccount)
        {
            string _lname;
            _lname = GetUserInfo(UserAccount, "sn", "samaccountname");
            return _lname;
        }

        public string GetDisplayNamefromUserID(string UserAccount)
        {
            string _dname;
            _dname = GetUserInfo(UserAccount, "displayName", "samaccountname");
            return _dname;
        }


        public string GetEmailIDfromUserID(string UserAccount)
        {
            return (GetUserInfo(UserAccount, "mail", "samaccountname"));
        }

        public string GetManagerNamefromUserID(string UserAccount)
        {
            string a = GetUserInfo(UserAccount, "Manager", "samaccountname");
            if (a != null)
                return (a.Substring(a.IndexOf("=") + 1, (System.Convert.ToInt32(a.IndexOf(",")) - System.Convert.ToInt32(a.IndexOf("=") + 1))));
            else return "";
        }

        public string GetManagerEmailfromUserID(string UserAccount)
        {
            string a = GetUserInfo(UserAccount, "Manager", "samaccountname");
            return (GetUserInfo(a, "mail", "distinguishedName"));
        }

        public string GetManagerIDfromUserID(string UserAccount)
        {
            string a = GetUserInfo(UserAccount, "Manager", "samaccountname");
            if (a != null)
                return (GetUserInfo(a, "samaccountname", "distinguishedName"));
            else
                return null;
        }

        public string GetPSNofromUserID(string UserAccount)
        {
            return (GetUserInfo(UserAccount, "samaccountname", "samaccountname"));
        }

        public string GetPSNoFromEmail(string UserAccount)
        {
            return (GetUserInfo(UserAccount, "SamAccountName", "mail"));
        }

        public string GetManagerNameFromEmail(string UserAccount)
        {
            string a = GetUserInfo(UserAccount, "Manager", "mail");
            return (a.Substring(a.IndexOf("=") + 1, (System.Convert.ToInt32(a.IndexOf(",")) - System.Convert.ToInt32(a.IndexOf("=") + 1))));
        }

        public string GetFullNameFromEmail(string UserAccount)
        {
            return (GetUserInfo(UserAccount, "givenName", "mail") + " " + GetUserInfo(UserAccount, "sn", "mail"));
        }

        public string GetPSNoFromname(string UserAccount)
        {
            return (GetUserInfo(UserAccount, "samaccountname", "CN"));
        }

        public string GetEmailFromName(string UserAccount)
        {
            return (GetUserInfo(UserAccount, "mail", "CN"));
        }

        public string GetPrincipalNameFromUserID(string UserAccount)
        {
            return (GetUserInfo(UserAccount, "userPrincipalName", "samaccountname"));
        }

        public string GetDepartmentFromUserID(string UserAccount)
        {
            return (GetUserInfo(UserAccount, "department", "samaccountname"));
        }

        public string GetDepartmentNameFromUserID(string UserAccount)
        {
            return GetUserInfo(UserAccount, "physicalDeliveryOfficeName", "samaccountname");
        }

        public string GetDepartmentFromName(string UserAccount)
        {
            return (GetUserInfo(UserAccount, "department", "CN"));
        }

        public string GetDepartmentFromEmail(string UserAccount)
        {
            return (GetUserInfo(UserAccount, "department", "mail") + " " + GetUserInfo(UserAccount, "sn", "mail"));
        }

        public string GetMobileNoFromUserID(string UserAccount)
        {
            string s = GetUserInfo(UserAccount, "mobile", "samaccountname");
            if (s != null)
                return s.Substring(s.Length - 10, 10);
            else
                return "";

        }

        public string GetDesignationFromUserID(string UserAccount)
        {
            return GetUserInfo(UserAccount, "title", "samaccountname");
        }




        public string SendEmail(string _frm, string _to, string _ccto, string _sub, string _bdy)
        {
            string rval = "";
            rval = SendMail(_frm, _to, _ccto, "", _sub, _bdy, "", "");
            return rval;
        }

        public string SendMail(Mail mail) {

            return SendMail(mail._from, mail._to, mail._ccto, mail._bccto, mail._subject, mail._body, mail._header, mail._sign);
        }

        public string SendMail(string _from, string _to, string _ccto, string _bccto, string _subject, string _body, string _header, string _sign)
        {
            string rval = "success";
            try
            {
                MailMessage mm = new MailMessage(_from, _to);

                mm.Subject = _subject;
                if (!string.IsNullOrEmpty(_ccto.Trim()))
                    mm.CC.Add(new MailAddress(_ccto));
                if (!string.IsNullOrEmpty(_bccto.Trim()))
                    mm.Bcc.Add(new MailAddress("panchaln@kecrpg.com"));
                
                mm.Body = _header + "<BR>" + _body + "<BR>" + _sign;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["smtp"].ToString();
                smtp.Port = 25;

                smtp.Send(mm);
            }
            catch (Exception ex)
            {
                rval = "failure:" + ex.ToString();
            }
            Console.Write("Mail output is:" + rval);
            return rval;
        }


        public bool getAuthentication(string uid, string pwd)
        {
            bool b = false;
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                b = ctx.ValidateCredentials(uid, pwd);
            }
            catch (Exception ex)
            {
                b = false;
            }
            return b;
        }

        public string sendSMS(string mobileno, string from, string txt) {
            string s = string.Empty;
            //string apiurl = "http://bhashsms.com/api/sendmsg.php?user=kecrpg&pass=********&sender=Sender ID
            //&phone=Mobile No&text=Test SMS&priority=Priority&stype=smstype
            string apiurl = ConfigurationManager.AppSettings["_smsuri"].ToString();
            string apiuser = ConfigurationManager.AppSettings["_smsid"].ToString();
            string apipwd = ConfigurationManager.AppSettings["_smspwd"].ToString();
            s = apiurl + "?user=" + apiuser + "&pass=" + apipwd + "&sender=" + from + "&phone=" + mobileno + "&text=" + txt + "&priority=sdnd&stype=normal";
            return callURI(s);
        }

        private string callURI(string u)
        {
            string s = string.Empty;
            Uri targetURI = new Uri(u);
            //HttpWebRequest wr = null;
            try
            {
                HttpWebRequest wr = WebRequest.Create(targetURI) as HttpWebRequest;

                if (wr.GetResponse().ContentLength > 0)
                {
                    StreamReader str = new StreamReader(wr.GetResponse().GetResponseStream());
                    s = str.ReadToEnd();
                    str.Close();
                }
                else
                {
                    s = "";
                }
            }
            catch (Exception ex)
            {
                s = ex.ToString();
            }

            return s;
        }


        public string changePassword(string uid, string pwd, string npwd)
        {
            string s = "";
            try
            {
                using (var Context = new PrincipalContext(ContextType.Domain))
                {
                    using (var User = UserPrincipal.FindByIdentity(Context, IdentityType.SamAccountName, uid))
                    {
                        // User.SetPassword(npwd)
                        User.ChangePassword(pwd, npwd);
                    }
                }
                s = "Password changed successfully.";
            }
            catch (Exception ex)
            {
                s = ex.Message;
            }


            return s;
        }


        public Person GetEmployeeDataFromUserID(string UserAccount)
        {
            Person Employee = new Person();
            Employee.EMNo = UserAccount;
            Employee.EmpCode = GetEmpCodeFromUserID(UserAccount);
            Employee.Name = GetDisplayNamefromUserID(UserAccount);
            Employee.Email = GetEmailIDfromUserID(UserAccount);
            Employee.Extn = "";
            Employee.Mobile = GetMobileNoFromUserID(UserAccount);
            Employee.DeptCode = GetDepartmentFromUserID(UserAccount);
            Employee.DeptName = GetDepartmentNameFromUserID(UserAccount);
            Employee.Designation = GetDesignationFromUserID(UserAccount);
            Employee.SBU = ""; // GetSBUFromUserID(UserAccount, 450);
            Employee.ManagerPSNo = GetManagerIDfromUserID(UserAccount);
            if (Employee.ManagerPSNo != null)
            {
                Employee.ManagerName = GetManagerNamefromUserID(UserAccount);
                Employee.ManagerEmail = GetManagerEmailfromUserID(UserAccount);
            }
            else {
                Employee.ManagerPSNo = "";
                Employee.ManagerName = "";
                Employee.ManagerEmail = "";
            }
            return Employee;
        }

        public ArrayList GetUsersList(string inSAM)
        {
            ArrayList rval = new ArrayList();
            // Public Function GetUserInfo(ByVal inSAM As String, ByVal inType As String) As String
            try
            {
                string SamAccount = inSAM; // Strings.Right(inSAM, Strings.Len(inSAM) - Strings.InStr(inSAM, @"\"));
                DirectorySearcher mySearcher = new DirectorySearcher(myDirectory);
                SearchResultCollection mySearchResultColl;
                SearchResult mySearchResult;
                ResultPropertyCollection myResultPropColl;
                ResultPropertyValueCollection myResultPropValueColl;
                // mySearcher.Filter = ("(&(objectClass=user)(" & attr & "=" & SamAccount & "))")
                if (inSAM.Length != 0)
                {
                    mySearcher.Filter = ("(&(objectCategory=person)(cn=*" + inSAM + "*) (!(userAccountControl:1.2.840.113556.1.4.803:=2))) ");

                    mySearchResultColl = mySearcher.FindAll();
                    // GetUserInfo(UserAccount, "givenName", "mail") & " " & GetUserInfo(UserAccount, "sn", "mail"))
                    if (mySearchResultColl.Count > 0)
                    {
                        // For value As Integer = 0 To mySearchResultColl.Count - 1
                        for (int value = 0; value <= 25; value++)
                        {

                            mySearchResult = mySearchResultColl[0];
                            myResultPropColl = mySearchResult.Properties;
                            myResultPropValueColl = myResultPropColl["cn"];
                            rval.Add(System.Convert.ToString(myResultPropValueColl[0]));


                        }
                    }
                }
                else
                    rval.Add("Enter Search Criteria");

                return rval;
            }
            catch (Exception ex)
            {
                rval.Add(ex.ToString());
                return rval;
            }
        }

        public class Mail
        {
            public string _from;
            public string _to;
            public string _ccto;
            public string _bccto;
            public string _subject;
            public string _body;
            public string _header;
            public string _sign;
        }

        public class Person
        {
            public string EMNo;
            public string Name;
            public string Email;
            public string Extn;
            public string Mobile;
            public string DeptCode;
            public string DeptName;
            public string Designation;
            public string SBU;
            public string ManagerPSNo;
            public string ManagerName;
            public string ManagerEmail;
            public string EmpCode;
        }

        public class SMS {
            public string mobileno;
            public string from;
            public string txt;
        }
    }
}