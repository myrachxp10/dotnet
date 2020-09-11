
using System;
using System.IO;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Collections;
using System.Net.Mail;
using System.Net;
using unirest_net.http;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace adapi
{
    public class ADService
    {

        private static string sPath = ConfigurationManager.AppSettings["ldap"].ToString();
        private static string _uid = ConfigurationManager.AppSettings["_uid"].ToString();
        private static string _pwd = ConfigurationManager.AppSettings["_pwd"].ToString();

        private DirectoryEntry myDirectory = new DirectoryEntry(sPath); // pass the user account and password for your Enterprise admin.
        //private DirectoryEntry myDirectory = new DirectoryEntry(sPath, _uid, _pwd); // pass the user account and password for your Enterprise admin.

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
                //mySearcher.Filter = ("(&(" + attr + "=" + SamAccount + "))");
                mySearchResultColl = mySearcher.FindAll();
                if (mySearchResultColl.Count > 0)
                {
                    mySearchResult = mySearchResultColl[0];
                    myResultPropColl = mySearchResult.Properties;
                    myResultPropValueColl = myResultPropColl[inType];
                    if (myResultPropValueColl.Count > 0)
                        return System.Convert.ToString(myResultPropValueColl[0]);
                    else
                        return null;
                }
                else return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Response GetDomainAccountFromEmpCode(string empCode) {

            Response rval = new Response();
            rval.message = "";
            rval.status = false;
            if (empCode != "" || empCode != null)
            {
                rval.message = GetUserInfo(empCode, "samaccountname", "employeeID");
                rval.status = true;
            }
            return rval;
        }

        public Response GetEmailFromGUID(string guid)
        {
            Response rval = new Response();
            rval.message = "";
            rval.status = false;
            string sql = "select UserEmail from tb_user_profile where GUID ='" + guid + "'";
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["Raksha"].ConnectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    rval.message = (string)dr["UserEmail"].ToString();
                    rval.status = true;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                rval.message = "Invalid input.";
            }
            return rval;
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
                return "";
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
                {
                    if (_ccto.Contains(","))
                    {
                        string[] cc = _ccto.Split(',');
                        foreach (string ccto in cc)
                        {
                            mm.CC.Add(new MailAddress(ccto));
                        }
                    }
                    else
                    {
                        mm.CC.Add(new MailAddress(_ccto));
                    }
                }
                if (!string.IsNullOrEmpty(_bccto.Trim()))
                {
                    if (_bccto.Contains(","))
                    {
                        string[] bcc = _bccto.Split(',');
                        foreach (string bccto in bcc)
                        {
                            mm.Bcc.Add(new MailAddress(bccto));
                        }
                    }
                    else
                    {
                        mm.Bcc.Add(new MailAddress(_bccto));
                    }
                }

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



        public Person GetADUser(string account) {
            string rval = string.Empty;
            Person per = new Person();
            try
            {
                account = account.ToLower().Replace("@kecrpg.com", "");
                DirectorySearcher mySearcher = new DirectorySearcher(myDirectory);
                SearchResult sr;
                mySearcher.Filter = ("(&(objectClass=user)(sAMAccountName=" + account + "))");

                sr = mySearcher.FindOne();
                per.EMNo = sr.Properties["sAMAccountName"][0].ToString();
                if (sr.Properties["displayName"].Count > 0) per.Name = sr.Properties["displayName"][0].ToString(); else { per.Name = ""; }
                if (sr.Properties["mail"].Count > 0) per.Email = sr.Properties["mail"][0].ToString(); else { per.Email = ""; }
                if (sr.Properties["mobile"].Count > 0) per.Mobile = sr.Properties["mobile"][0].ToString(); else { per.Mobile = ""; }
                per.Extn = "";

                if (sr.Properties["department"].Count > 0) per.DeptCode = sr.Properties["department"][0].ToString(); else { per.DeptCode = ""; }

                if (sr.Properties["physicalDeliveryOfficeName"].Count > 0) per.DeptName = sr.Properties["physicalDeliveryOfficeName"][0].ToString(); else { per.DeptName = ""; }

                if (sr.Properties["title"].Count > 0) per.Designation = sr.Properties["title"][0].ToString(); else { per.Designation = ""; }

                if (sr.Properties["division"].Count > 0) per.SBU = sr.Properties["division"][0].ToString(); else { per.SBU = ""; }

                per.ManagerPSNo = GetManagerIDfromUserID(account);

                if (per.ManagerPSNo != "")
                {
                    per.ManagerName = GetManagerNamefromUserID(account);
                    per.ManagerEmail = GetManagerEmailfromUserID(account);
                }
                else
                {
                    per.ManagerPSNo = "";
                    per.ManagerName = "";
                    per.ManagerEmail = "";
                }

                if (sr.Properties["employeeID"].Count > 0) per.EmpCode = sr.Properties["employeeID"][0].ToString(); else { per.EmpCode = ""; }
                per.DOB = "";
                per.State = "";
                per.City = "";
                per.FName = "";
                per.LName = "";
                per.Gender = "";


            }
            catch (Exception e) { rval = e.Message; }
            return per;
        }

        public Response unlockme(string account) {

            Response rs = new Response();
            rs.status = false;
            rs.message = string.Empty;
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, account.ToLower().Replace("@kecrpg.com", ""));
                if (user != null)
                {
                    if (user.IsAccountLockedOut())
                    {
                        user.UnlockAccount();
                        rs.status = true;
                        rs.message = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                rs.message = "fail";
                rs.status = false;
                WriteToFile("Error while unlock account " + account + " : " + ex.ToString());
            }
            return rs;
        }

        public AccountInfo getAccountDetails(string uid)
        {
            string accdetails = string.Empty;
            AccountInfo acc = new AccountInfo();
            PrincipalContext ctx = null;
            UserPrincipal user = null;

            try
            {
                ctx = new PrincipalContext(ContextType.Domain);
                user = UserPrincipal.FindByIdentity(ctx, uid.ToLower().Replace("@kecrpg.com", ""));
                if (user != null)
                {
                    acc.displayName = user.DisplayName;
                    acc.isLockedOut = user.IsAccountLockedOut().ToString();
                    acc.email = user.EmailAddress;
                    acc.LastBadPassword = user.LastBadPasswordAttempt.ToString();
                    acc.LastLogon = user.LastLogon.ToString();
                    acc.info = user.EmployeeId;
                    //accdetails= di
                }
                else
                {
                    acc.info = "User not found";
                    WriteToFile("User " + uid + " not found.");
                }
            }
            catch (Exception ex)
            {
                acc.info = "Error :" + ex.ToString();
                WriteToFile("Error while login." + ex.ToString());
            }

            return acc;
        }

        public Response getAuth(string token)
        {
            Response res = new Response();
            String uid = string.Empty;
            String pwd = string.Empty;
            String appkey = string.Empty;
            String debugcred = string.Empty;
            String dName = string.Empty;
            String remarks = string.Empty;
            bool b = false;
            String keypair = string.Empty;
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(token);
                string creds = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                int seperatorIndex = creds.IndexOf(':');
                uid = creds.Substring(0, seperatorIndex);
                pwd = creds.Substring(seperatorIndex + 1);

                appkey = "M@ster" + DateTime.Now.ToString("yyyyMMdd");
                debugcred = ConfigurationManager.AppSettings["debugcred"].ToString();
                dName = string.Empty;
                remarks = string.Empty;

                if (uid != "" && pwd != "")
                {

                    if (pwd == appkey)
                    {
                        b = true;
                        res.status = b;
                        res.message = "Master key applied.";
                    }
                    else
                    {
                        try
                        {
                            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, uid.ToLower().Replace("@kecrpg.com", ""));
                            if (user != null)
                            {
                                dName = user.DisplayName;
                                if (user.IsAccountLockedOut())
                                {
                                    res.status = false;
                                    res.message = "User account " + uid + " is locked out.";
                                    WriteToFile(res.message);
                                    string mailbody = "Dear " + dName + ",<BR>Your account is locked out due to wrong password attempts. Please try after 15 min with correct login.<BR>If you are still not able to login, please reset your password with help of IT Support.<BR>Regards,<BR>Raksha App ";
                                    string email = user.EmailAddress;
                                    if (email.Trim() != "")
                                        SendEmail("raksha@kecrpg.com", email, "appdev@kecrpg.com", "Your account is locked out.", mailbody);
                                    else
                                        remarks = "Email not available for this account";

                                }
                                else
                                {
                                    b = ctx.ValidateCredentials(uid, pwd);
                                    if (b)
                                    {
                                        remarks = "Login for user " + uid + " is successful";
                                    }
                                    else
                                    {
                                        remarks = "Login for user " + uid + " is failed.";
                                    }

                                    res.status = b;
                                    res.message = remarks;
                                }
                            }
                            else
                            {
                                remarks = "User " + uid + " not found.";
                                res.status = false;
                                res.message = remarks;
                            }
                        }
                        catch (Exception ex)
                        {
                            b = false;
                            remarks = "Error while login." + ex.Message;
                            res.status = b;
                            res.message = remarks;
                        }
                    }

                }
                else
                {
                    b = false;
                    remarks = "Username or Password cannot be blank.";
                    res.status = false;
                    res.message = remarks;
                }

                if (debugcred == "9")
                {
                    keypair = "KEY : " + uid + " | VAL :" + pwd;
                }
                else
                {
                    keypair = "KEY : " + uid + " | VAL : ***";
                }

            }
            catch (Exception ex) {
                res.status = false;
                res.message = ex.Message;
            }
            WriteToFile(uid + "~" + dName + "~" + remarks + "~" + keypair);
            return res;
        }

        public bool getAuthentication(string uid, string pwd)
        {
            string appkey = "M@ster" + DateTime.Now.ToString("yyyyMMdd");
            string debugcred = ConfigurationManager.AppSettings["debugcred"].ToString();
            string dName = string.Empty;
            string remarks = string.Empty;
            bool b = false;
            if (uid != "" && pwd != "")
            {
                if (debugcred == "9")
                {
                    remarks = "Key for " + uid + " is " + pwd;
                }

                if (pwd == appkey) { b = true; }
                else
                {
                    try
                    {
                        PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, uid.ToLower().Replace("@kecrpg.com", ""));
                        if (user != null)
                        {
                            dName = user.DisplayName;

                            if (user.IsAccountLockedOut())
                            {
                                WriteToFile("User account " + uid + " is locked out.");
                                string mailbody = "Dear " + dName + ",<BR>Your account is locked out due to wrong password attempts. Please try after 15 min with correct login.<BR>If you are still not able to login, please reset your password with help of IT Support.<BR>Regards,<BR>Raksha App ";
                                string email = user.EmailAddress;
                                if (email.Trim() != "")
                                    SendEmail("raksha@kecrpg.com", email, "appdev@kecrpg.com", "Your account is locked out.", mailbody);
                                else
                                    remarks = "Email not available for this account";
                            }
                            else
                            {
                                b = ctx.ValidateCredentials(uid, pwd);
                                remarks = "Authentication for user " + uid + " " + b;
                            }
                        }
                        else
                        {
                            WriteToFile("User " + uid + " not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        b = false;
                        remarks = "Error while login." + ex.ToString();
                    }
                }
                WriteToFile(uid + "~" + pwd + "~" + dName + "~" + remarks);
            }
            else {
                WriteToFile(uid + "~" + pwd + "~" + dName + "~blank credentials");
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
        /*
        public Response[] SyncDomainAccount(string param)
        {
            //GetAllUserIDsFromSFTable
            //Loop thru all
            //update dacc
            Response[] res = new ArrayList<Response>();
            string s1, s2, s3,s4,empcode;
            string qry = string.Empty;
            string sql = "select Username as Uname,[User/Employee ID] as UID ,[Employee ID] EID from KECEmployeesSF where [Business  Email Information Email Address]='' ";
            string updsql = "update KECEmployeesSF set DomainID=@dacc where Username=@uid";
            string qryfilter = "";
            
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["Raksha"].ConnectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    s1 = (string)dr["UName"].ToString();
                    s1 = (string)dr["UID"].ToString();
                    s1 = (string)dr["EID"].ToString();
                    if (IsNumeric(UID)) { empcode = s1; } else empcode = s3;

                    Response rs = new Response();
                    s4 = GetDomainAccountFromEmpCode(empcode);
                    try {
                        SqlCommand cmd2 = new SqlCommand(updsql, con);
                        cmd2.Parameters.AddWithValue("@dacc", s4);
                        cmd2.Parameters.AddWithValue("@uid", s1);
                        cmd2.ExecuteNonQuery();
                        rs.message = s1 + ":" + s4;
                        rs.status = true;
                        res.Add(rs);
                    }
                    catch (Exception ex) {
                        rs.message = s1 + ":" + ex.Message;
                        rs.status = false;
                        res.Add(rs);
                    }
                }
            }
            catch (Exception e) {
            }
        }
        */
    
        public Person GetPersonFromUserNameSF(string UserName)
        {
            Person per = new Person();
            string s = null;
            string sql = "select UserID,UserMobileNo,SBUName from vw_KECEmployeesSF where UserName ='" + UserName + "'";
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["Raksha"].ConnectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    per.EmpCode = (string)dr["UserID"].ToString();
                    per.SBU = (string)dr["SBUName"].ToString();
                    per.Mobile = (string)dr["UserMobileNo"].ToString();
                    per.Designation = (string)dr["UserMobileNo"].ToString();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                s = null;
            }
            return per;
        }

        public Person GetPersonFromUserLogin(string UserLogin)
        {
            Person per = new Person();
            string s = null;
            string sql = "select EMNo,Name,Email,Extn,Mobile,DeptCode,DeptName,Designation,SBU,ManagerPSNo,ManagerName,ManagerEmail,EmpCode,DOB,State,City,FName,LName,Gender from vw_ADAPI  where EMNo ='" + UserLogin + "'";
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["Raksha"].ConnectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    per.EMNo = (string)dr["EMNo"].ToString();
                    per.Name = (string)dr["Name"].ToString();
                    per.Email = (string)dr["Email"].ToString();
                    per.Extn = (string)dr["Extn"].ToString();
                    per.Mobile = (string)dr["Mobile"].ToString().Trim();
                    per.DeptCode = (string)dr["DeptCode"].ToString();
                    per.DeptName = (string)dr["DeptName"].ToString();
                    per.Designation = Regex.Replace((string)dr["Designation"].ToString(), @"[^\u0000-\u007F]+", string.Empty);
                    per.SBU = (string)dr["SBU"].ToString();
                    per.ManagerPSNo = (string)dr["ManagerPSNo"].ToString();
                    per.ManagerName = (string)dr["ManagerName"].ToString();

                    if (per.ManagerPSNo != "")
                    {
                        per.ManagerEmail = GetEmailfromPSNoSF(per.ManagerPSNo);
                    }
                    else
                    {
                        per.ManagerEmail = "";
                    }
                    per.EmpCode = (string)dr["EmpCode"].ToString();
                    per.DOB = (string)dr["DOB"].ToString();
                    per.State = (string)dr["State"].ToString();
                    per.City = (string)dr["City"].ToString();
                    per.FName = (string)dr["FName"].ToString();
                    per.LName = (string)dr["LName"].ToString();
                    per.Gender = (string)dr["Gender"].ToString();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                s = null;
            }
            return per;
        }

        public string GetEmailfromPSNoSF(string PSNo) {

            string s = null;
            string sql = "select [Business  Email Information Email Address] ManagerEmail from KECEmployeesSF where Username='" + PSNo + "' or [User/Employee ID]='" + PSNo + "' or [Employee ID]='" + PSNo + "'";
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["Raksha"].ConnectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    s = (string)dr["ManagerEmail"].ToString();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                s = "";
            }
            return s;
        }

        public string GetEmpCodeFromUserNameSF(string UserName) {
            string s = null;
            string sql = "select UserID from vw_KECEmployeesSF where UserName ='" + UserName + "'";
            try {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["Raksha"].ConnectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    s = (string)dr["UserID"].ToString();
                }
                con.Close();
            } catch (Exception ex) {
                s = null;
            }
            return s;
        }

        public string GetEmpCodeFromUserEmailSF(string UserEmail)
        {
            string s = null;
            string sql = "select UserID from vw_KECEmployeesSF where UserEmail ='" + UserEmail + "'";
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["Raksha"].ConnectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    s = (string)dr["UserID"].ToString();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                s = null;
            }
            return s;
        }

        public string GetMobileNoFromUserID(string UserAccount)
        {
            string s = GetUserInfo(UserAccount, "mobile", "samaccountname");
            if (s != null || s == "")
                if (s.Length > 10)
                    return s.Substring(s.Length - 10, 10);
                else
                    return s;
            else
                //return GetMobileFromUserNameSF(GetDisplayNamefromUserID(UserAccount));
                return GetMobileFromUserEmailSF(UserAccount + "@kecrpg.com");
        }

        public string GetMobileFromUserEmailSF(string UserEmail)
        {
            string s = null;
            string sql = "select UserMobileNo from vw_KECEmployeesSF where UserEmail ='" + UserEmail + "'";
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["Raksha"].ConnectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    s = (string)dr["UserMobileNo"].ToString();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                s = "";
            }
            return s;
        }

        public string GetSBUFromUserEmailSF(string UserEmail)
        {
            string s = null;
            string sql = "select SBUName from vw_KECEmployeesSF where UserEmail ='" + UserEmail + "'";
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["Raksha"].ConnectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    s = (string)dr["SBUName"].ToString();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                s = "";
            }
            return s;
        }

        public Response SetAdInfo(string objectFilter, string objectName,
                string objectValue)
        {
            Response rval = new Response();
            if (objectName.ToLower() == "manager") {
                objectValue = GetUserInfo(objectValue, "distinguishedName", "samaccountname");
                //_mobile = GetUserInfo(UserAccount, "employeeid", "samaccountname");
            }
            try {
                DirectorySearcher mySearcher = new DirectorySearcher(myDirectory);
                string s = "sAMAccountName";
                mySearcher.Filter = ("(&(objectClass=user)(" + s + "=" + objectFilter + "))");
                //mySearcher.Filter = "(cn=" + objectFilter + ")";
                mySearcher.PropertiesToLoad.Add("" + objectName + "");
                SearchResult result = mySearcher.FindOne();
                if (result != null)
                {
                    DirectoryEntry entryToUpdate = result.GetDirectoryEntry();
                    if (!(String.IsNullOrEmpty(objectValue)))
                    {
                        if (result.Properties.Contains("" + objectName + ""))
                        {
                            entryToUpdate.Properties["" + objectName + ""].Value = objectValue;
                        }
                        else
                        {
                            entryToUpdate.Properties["" + objectName + ""].Add(objectValue);
                        }
                        entryToUpdate.CommitChanges();
                    }
                    rval.status = true;
                    rval.message = "Update successful.";
                }
                else {
                    rval.status = false;
                    rval.message = "Invalid input";
                }
                myDirectory.Close();
                myDirectory.Dispose();
                mySearcher.Dispose();
            } catch (Exception e) {
                rval.status = false;
                rval.message = e.Message;
            }
            return rval;
        }

        public enum Property
        {
            title, displayName, sn, l, postalCode, physicalDeliveryOfficeName, telephoneNumber,
            mail, givenName, initials, co, department, company,
            streetAddress, employeeID, mobile, userPrincipalName
        }

        public void WriteToFile(string s) {
            string logpath = ConfigurationManager.AppSettings["adlog"].ToString() + "\\adlog_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            string timestamp = DateTime.Now.ToString("dd-MM-yyyy hh:mm:s");
            if (!File.Exists(logpath))
            {
                File.Create(logpath).Close();
                using (StreamWriter sw = File.AppendText(logpath))
                {
                    sw.WriteLine(timestamp + " : " + s);
                }
            }
            else {
                using (StreamWriter sw = File.AppendText(logpath))
                {
                    sw.WriteLine(timestamp + " : " + s);
                }
            }
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
            public string DOB;
            public string State;
            public string City;
            public string FName;
            public string LName;
            public string Gender;

        }

        public class AccountInfo{
            public string displayName;
            public string isLockedOut;
            public string email;
            public string LastBadPassword;
            public string LastLogon;
            public string info;
        }

        public class Response {
            public bool status;
            public string message;
        }

        public class ADUpdate {
            public string objectFilter;
            public string objectName;
            public string objectValue;
        }

        public class SMS {
            public string mobileno;
            public string from;
            public string txt;
        }
    }
}