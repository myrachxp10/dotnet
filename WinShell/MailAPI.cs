using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Outlook;

namespace adapi
{
    class MailAPI
    {
        

        public static void ReadMailItems()
        {
            Application outlookApplication = new Application();
            NameSpace outlookNamespace = outlookApplication.GetNamespace("MAPI");
            MAPIFolder inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
            Items mailItems = inboxFolder.Items;

            try
            {
                outlookApplication = new Application();
                outlookNamespace = outlookApplication.GetNamespace("MAPI");
                inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
                mailItems = inboxFolder.Items;

                foreach (MailItem item in mailItems)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("From: " + item.SenderEmailAddress);
                    stringBuilder.AppendLine("To: " + item.To);
                    stringBuilder.AppendLine("CC: " + item.CC);
                    stringBuilder.AppendLine("");
                    stringBuilder.AppendLine("Subject: " + item.Subject);
                    stringBuilder.AppendLine(item.Body);

                    Console.WriteLine(stringBuilder);
                    Marshal.ReleaseComObject(item);
                }
            }
            catch { }
            finally
            {
                ReleaseComObject(mailItems);
                ReleaseComObject(inboxFolder);
                ReleaseComObject(outlookNamespace);
                ReleaseComObject(outlookApplication);
            }
        }

        private static void ReleaseComObject(object obj)
        {
            if (obj != null)
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
        }
    }
}
