using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GuerrillaMailExample
{
    class GuerrillaMail : IDisposable
    {
        /* 
         * TmpMail
         * -------------------------------------------------------------
         * Quick and easy Temp email class for GuerrillaMail
         * Get new email, get messages, send messages then dispose of it
         * 
         * Free to use for whatever purpose
         * https://github.com/Ezzpify/
         *
        */


        /// <summary>
        /// Class variables
        /// </summary>
        private string _sHost = "http://api.guerrillamail.com/ajax.php?";
        private string _wProxy;
        private CookieContainer _cCookies;


        /// <summary>
        /// Email variables
        /// </summary>
        private string _sTimeStamp;
        private string _sAlias;
        private string _sSidToken;


        /// <summary>
        /// Initializer for the class.
        /// Provide a (string)proxy if the requests should go via a proxy connection
        /// </summary>
        /// <param name="Proxy"></param>
        public GuerrillaMail(string proxy = "")
        {
            /*If we got passed a Proxy variable*/
            if (!string.IsNullOrEmpty(proxy))
            {
                /*Regex to match a proxy*/
                string ValidIPRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]):[\d]+$";
                if (Regex.IsMatch(proxy, ValidIPRegex))
                {
                    /*Proxy was in a valid format*/
                    _wProxy = proxy;
                }
            }

            /*Initialize stuff*/
            _cCookies = new CookieContainer();

            /*Initialize the email*/
            JObject Obj = JObject.Parse(Contact("f=get_email_address"));
            _sTimeStamp = (string)Obj.SelectToken("email_timestamp");
            _sAlias = ((string)Obj.SelectToken("email_addr")).Split('@')[0];
            _sSidToken = (string)Obj.SelectToken("sid_token");

            /*Delete the automatic welcome email - id is always 1*/
            DeleteSingleEmail("1");
        }


        /// <summary>
        /// Returns all emails in a json string
        /// offset=0 implies getting all emails
        /// </summary>
        /// <returns></returns>
        public string GetAllEmails()
        {
            return JObject.Parse(Contact("f=get_email_list&offset=0")).SelectToken("list").ToString(Formatting.Indented);
        }


        /// <summary>
        /// Returns all emails received after a specific email (specified by mail_id)
        /// Example: GetEmailsSinceID(53451833)
        /// </summary>
        /// <param name="mail_id"></param>
        /// <returns></returns>
        public string GetEmailsSinceID(string mail_id)
        {
            return JObject.Parse(Contact("f=check_email&seq=" + mail_id)).SelectToken("list").ToString(Formatting.Indented);
        }


        /// <summary>
        /// Returns the last email
        /// If there are no emails it will return empty string
        /// </summary>
        /// <returns></returns>
        public string GetLastEmail()
        {
            /*Get all emails and select index 0*/
            JObject Emails = JObject.Parse(Contact("f=get_email_list&offset=0"));
            JObject LastEmail = (JObject)Emails.SelectToken("list[0]");

            /*Null check*/
            if (LastEmail != null)
            {
                /*Return last email if it is not null*/
                return LastEmail.ToString(Formatting.Indented);
            }
            else
            {
                /*last email is null, return empty string*/
                return "";
            }
        }


        /// <summary>
        /// Returns our email
        /// domain 0 or 1
        /// </summary>
        /// <returns></returns>
        public string GetMyEmail(int domain = 0)
        {
            /*Email adress string*/
            string Email = string.Format("{0}@", _sAlias);

            /*There are several email domains you can use by default*/
            switch (domain)
            {
                case 1:
                    return Email + "grr.la";
                case 2:
                    return Email + "guerrillamail.biz";
                case 3:
                    return Email + "guerrillamail.com";
                case 4:
                    return Email + "guerrillamail.de";
                case 5:
                    return Email + "guerrillamail.net";
                case 6:
                    return Email + "guerrillamail.org";
                case 7:
                    return Email + "guerrillamailblock.com";
                case 8:
                    return Email + "spam4.me";
                default:
                    return Email + "sharklasers.com";
            }
        }


        /// <summary>
        /// Deletes an array of emails from the mailbox
        /// </summary>
        /// <param name="mail_ids"></param>
        public void DeleteEmails(string[] mail_ids)
        {
            /*If there are at least 1 ID in the array*/
            if (mail_ids.Length > 0)
            {
                /*Go through each array value and format delete string*/
                string IDString = string.Empty;
                foreach (string id in mail_ids)
                {
                    /*Example: &email_ids[]53666&email_ids[]53667*/
                    IDString += string.Format("&email_ids[]{0}", id);
                }

                /*Delete the emails*/
                Contact("f=del_email" + IDString);
            }
        }


        /// <summary>
        /// Deletes a single email
        /// </summary>
        /// <param name="mail_id"></param>
        public void DeleteSingleEmail(string mail_id)
        {
            Contact("f=del_email&email_ids[]=" + mail_id);
        }


        /// <summary>
        /// Contacts the specified host(url) to get information
        /// Calling this method refreshes all mail received etc
        /// </summary>
        /// <param name="url"></param>
        private string Contact(string parameters)
        {
            /*Set up the request*/
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_sHost + parameters);
            request.CookieContainer = _cCookies;
            request.Method = "GET";
            request.Host = "www.guerrillamail.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Proxy = new WebProxy(_wProxy);

            /*Fetch the response*/
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                /*Check if the response status is okay*/
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    /*Get the stream*/
                    using (Stream stream = response.GetResponseStream())
                    {
                        /*Get the full string*/
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        return reader.ReadToEnd();
                    }
                }
            }

            /*Something messed up, returning empty string*/
            return string.Empty;
        }


        /// <summary>
        /// Dispose object
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
