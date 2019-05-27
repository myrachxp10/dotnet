using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.DirectoryServices;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Collections;
using System.Net.Mail;

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
                            return "Null";
                            //return;
                        }

                    case object _ when mySearchResultColl.Count > 1:
                        {
                            return "Null";
                            //return;
                        }
                }
                mySearchResult = mySearchResultColl[0];
                myResultPropColl = mySearchResult.Properties;
                myResultPropValueColl = myResultPropColl[inType];
                return System.Convert.ToString(myResultPropValueColl[0]);
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
            return (a.Substring(a.IndexOf("=") + 1, (System.Convert.ToInt32(a.IndexOf(",")) - System.Convert.ToInt32(a.IndexOf("=") + 1))));
        }

        public string GetManagerEmailfromUserID(string UserAccount)
        {
            string a = GetUserInfo(UserAccount, "Manager", "samaccountname");
            return (GetUserInfo(a, "mail", "distinguishedName"));
        }

        public string GetManagerIDfromUserID(string UserAccount)
        {
            string a = GetUserInfo(UserAccount, "Manager", "samaccountname");
            return (GetUserInfo(a, "samaccountname", "distinguishedName"));
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
            return s.Substring(s.Length - 10, 10);

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

            Employee.Name = GetDisplayNamefromUserID(UserAccount);
            Employee.Email = GetEmailIDfromUserID(UserAccount);
            Employee.Extn = "";
            Employee.Mobile = GetMobileNoFromUserID(UserAccount);
            Employee.DeptCode = GetDepartmentFromUserID(UserAccount);
            Employee.DeptName = GetDepartmentNameFromUserID(UserAccount);
            Employee.Designation = GetDesignationFromUserID(UserAccount);
            Employee.SBU = ""; // GetSBUFromUserID(UserAccount, 450);
            Employee.ManagerPSNo = GetManagerIDfromUserID(UserAccount);
            Employee.ManagerName = GetManagerNamefromUserID(UserAccount);
            Employee.ManagerEmail = GetManagerEmailfromUserID(UserAccount);

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
        }
    }
}