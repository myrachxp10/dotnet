using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using Microsoft.SharePoint.Client;
using System.Configuration;
using System.IO;
using System.Net;

namespace adapi
{
    public class DMS
    {
        private static string spURL = ConfigurationManager.AppSettings["spURL"].ToString();
        private static string _uid = ConfigurationManager.AppSettings["spUser"].ToString();
        private static string _pwd = ConfigurationManager.AppSettings["spPWD"].ToString();

        public string UploadToDMS(dmsDoc doc) {
            return UploadToDMS(doc.fileByteString, doc._docLib, doc.folderName, doc.fileName);
        }

        public string UploadToDMS(string fileByteString, string _docLib, string folderName, string fileName)
        {

            string rval = string.Empty;
            byte[] tempBytes = Convert.FromBase64String(fileByteString);

            using (ClientContext clientContext = new ClientContext(spURL))
            {
                SecureString passWord = new SecureString();
                foreach (char c in _pwd.ToCharArray()) passWord.AppendChar(c);
                clientContext.Credentials = new SharePointOnlineCredentials(_uid, passWord);
                List list = clientContext.Web.Lists.GetByTitle(_docLib);
                clientContext.Load(list.RootFolder);
                var folders = list.RootFolder.Folders;
                string docURL = spURL + "/" + _docLib + "/" + folderName + "/" + fileName;
                clientContext.Load(folders);
                clientContext.ExecuteQuery();
                folders.Add(folderName);
                clientContext.ExecuteQuery();

                using (Stream stream = new MemoryStream(tempBytes))
                {
                    try
                    {
                        Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, list.RootFolder.ServerRelativeUrl.ToString() + "/" + folderName + "/" + fileName, stream, true);
                        rval = docURL;
                    }
                    catch (Exception ex)
                    {
                        rval = "Error occured: " + ex.ToString();
                    }

                }
            }
            return rval;
        }

        public string DeleteDOC(string _docURL)
        {
            ClientContext clientContext = new ClientContext(spURL);
            
            using (var sp = new ClientContext(spURL))
            {
                SecureString passWord = new SecureString();
                foreach (char c in _pwd.ToCharArray()) passWord.AppendChar(c);
                sp.Credentials = new SharePointOnlineCredentials(_uid, passWord);
               
                var file = sp.Web.GetFileByUrl(_docURL);
                sp.Load(file, f => f.Exists);
                file.DeleteObject();
                sp.ExecuteQuery();
                if (!file.Exists)
                    throw new System.IO.FileNotFoundException();
            }

            return "success";
        }

        public string OpenDoc(String doclink)
        {
            string rval = string.Empty;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_uid, _pwd);
                    string fileName = "";
                    string[] s = doclink.Split('/');
                    fileName = s[s.Length - 1];
                    client.DownloadFile(doclink, fileName);
                    rval = "done";
                }
            }
            catch (Exception ex)
            {
                rval = ex.ToString();
            }
            return rval;
        }

        public class dmsDoc {
            public string fileByteString;
            public string _docLib;
            public string folderName;
            public string fileName;
        }
    }
}