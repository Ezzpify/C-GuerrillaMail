using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

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
        /// Email content class
        /// </summary>
        public class EmailList
        {
            /// <summary>
            /// ID of email
            /// </summary>
            public object mail_id { get; set; }


            /// <summary>
            /// Address of sender
            /// </summary>
            public string mail_from { get; set; }


            /// <summary>
            /// Email subject
            /// </summary>
            public string mail_subject { get; set; }


            /// <summary>
            /// Email content
            /// </summary>
            public string mail_excerpt { get; set; }


            /// <summary>
            /// Email timestamp
            /// </summary>
            public object mail_timestamp { get; set; }


            /// <summary>
            /// If email is read
            /// This is 0 if unread
            /// </summary>
            public object mail_read { get; set; }


            /// <summary>
            /// Email received date GMT
            /// </summary>
            public string mail_date { get; set; }


            /// <summary>
            /// Email attribute
            /// </summary>
            public object att { get; set; }


            /// <summary>
            /// Email size kb
            /// </summary>
            public string mail_size { get; set; }


            /// <summary>
            /// Email replied to
            /// </summary>
            public string reply_to { get; set; }


            /// <summary>
            /// Content type
            /// Example: text
            /// </summary>
            public string content_type { get; set; }


            /// <summary>
            /// Email recipients
            /// </summary>
            public string mail_recipient { get; set; }


            /// <summary>
            /// Email source id
            /// </summary>
            public int? source_id { get; set; }


            /// <summary>
            /// Email source mail id
            /// </summary>
            public int? source_mail_id { get; set; }


            /// <summary>
            /// Email body
            /// </summary>
            public string mail_body { get; set; }


            /// <summary>
            /// Email size kb
            /// </summary>
            public int? size { get; set; }
        }


        /// <summary>
        /// Email stats class
        /// </summary>
        public class Stats
        {
            public string sequence_mail { get; set; }
            public int created_addresses { get; set; }
            public string received_emails { get; set; }
            public string total { get; set; }
            public string total_per_hour { get; set; }
        }


        /// <summary>
        /// Email auth class
        /// </summary>
        public class Auth
        {
            public bool success { get; set; }
            public List<object> error_codes { get; set; }
        }


        /// <summary>
        /// Main email root class
        /// </summary>
        public class Email
        {
            public List<EmailList> list { get; set; }
            public string count { get; set; }
            public string email { get; set; }
            public string alias { get; set; }
            public int ts { get; set; }
            public string sid_token { get; set; }
            public Stats stats { get; set; }
            public Auth auth { get; set; }
        }


        /// <summary>
        /// Cookie container storage
        /// </summary>
        private CookieContainer mCookies = new CookieContainer();


        /// <summary>
        /// Optional proxy address string
        /// </summary>
        private string mProxy;


        /// <summary>
        /// Email address name
        /// </summary>
        private string mEmailAlias;


        /// <summary>
        /// Initializer for the class.
        /// </summary>
        /// <param name="Proxy">Include a Proxy adress string if the request should go via a proxy</param>
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
                    mProxy = proxy;
                }
            }

            /*Initialize the email*/
            JObject Obj = JObject.Parse(Contact("f=get_email_address"));
            mEmailAlias = ((string)Obj.SelectToken("email_addr")).Split('@')[0];

            /*Delete the automatic welcome email - id is always 1*/
            DeleteSingleEmail("1");
        }


        /// <summary>
        /// Returns full json response
        /// </summary>
        /// <returns></returns>
        public Email GetContent()
        {
            return JsonConvert.DeserializeObject<Email>(Contact("f=get_email_list&offset=0"));
        }


        /// <summary>
        /// Returns all emails in a json string
        /// offset=0 implies getting all emails
        /// </summary>
        /// <returns>Returns list of email</returns>
        public List<EmailList> GetAllEmails()
        {
            var emails = JsonConvert.DeserializeObject<Email>(Contact("f=get_email_list&offset=0"));
            return emails.list;
        }


        /// <summary>
        /// Returns all emails received after a specific email (specified by mail_id)
        /// Example: GetEmailsSinceID(53451833)
        /// </summary>
        /// <param name="mail_id">mail_id of an email</param>
        /// <returns>Returns list of emails</returns>
        public List<EmailList> GetEmailsSinceID(string mail_id)
        {
            var emails = JsonConvert.DeserializeObject<Email>(Contact("f=check_email&seq=" + mail_id));
            return emails.list;
        }


        /// <summary>
        /// Returns the last email
        /// If there are no emails it will return empty string
        /// </summary>
        /// <returns>Returns null if no email</returns>
        public EmailList GetLastEmail()
        {
            var emails = JsonConvert.DeserializeObject<Email>(Contact("f=get_email_list&offset=0"));
            return emails.list.LastOrDefault();
        }


        /// <summary>
        /// Returns our email with a specified domain
        /// </summary>
        /// <param name="domain">Specifies which domain to return (0-8) useful for services that blocks certain domains</param>
        /// <returns>Returns email as string</returns>
        public string GetMyEmail(int domain = 0)
        {
            /*There are several email domains you can use by default*/
            /*Some sites may block guerrilla mail domains, so we can use several different ones*/
            string address = string.Format("{0}@", mEmailAlias);
            switch (domain)
            {
                case 1:
                    return address + "grr.la";
                case 2:
                    return address + "guerrillamail.biz";
                case 3:
                    return address + "guerrillamail.com";
                case 4:
                    return address + "guerrillamail.de";
                case 5:
                    return address + "guerrillamail.net";
                case 6:
                    return address + "guerrillamail.org";
                case 7:
                    return address + "guerrillamailblock.com";
                case 8:
                    return address + "spam4.me";

                default:
                    return address + "sharklasers.com";
            }
        }


        /// <summary>
        /// Deletes an array of emails from the mailbox
        /// </summary>
        /// <param name="mail_ids">String array of mail ids</param>
        public void DeleteEmails(string[] mail_ids)
        {
            /*If there are at least 1 ID in the array*/
            if (mail_ids.Length > 0)
            {
                /*Go through each array value and format delete string*/
                string idString = string.Empty;
                foreach (string id in mail_ids)
                {
                    /*Example: &email_ids[]53666&email_ids[]53667*/
                    idString += string.Format("&email_ids[]{0}", id);
                }

                /*Delete emails*/
                Contact("f=del_email" + idString);
            }
        }


        /// <summary>
        /// Deletes an array of emails from the mailbox
        /// </summary>
        /// <param name="mail_ids">List string of mail ids</param>
        public void DeleteEmails(List<string> mail_ids)
        {
            /*Format email delete string*/
            string idString = string.Empty;
            mail_ids.ForEach(o => idString += string.Format("&email_ids[]{0}", o));

            /*Delete emails*/
            Contact("f=del_email" + idString);
        }


        /// <summary>
        /// Deletes a single email
        /// </summary>
        /// <param name="mail_id">mail_id of an email</param>
        public void DeleteSingleEmail(string mail_id)
        {
            Contact("f=del_email&email_ids[]=" + mail_id);
        }using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

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
            /// Class to hold a proxy
            /// </summary>
            private class Proxy
            {
                public string address { get; set; }
                public int port { get; set; }
                public bool initialized { get; set; }
            }


            /// <summary>
            /// Email content class
            /// </summary>
            public class EmailList
            {
                /// <summary>
                /// ID of email
                /// </summary>
                public object mail_id { get; set; }


                /// <summary>
                /// Address of sender
                /// </summary>
                public string mail_from { get; set; }


                /// <summary>
                /// Email subject
                /// </summary>
                public string mail_subject { get; set; }


                /// <summary>
                /// Email content
                /// </summary>
                public string mail_excerpt { get; set; }


                /// <summary>
                /// Email timestamp
                /// </summary>
                public object mail_timestamp { get; set; }


                /// <summary>
                /// If email is read
                /// This is 0 if unread
                /// </summary>
                public object mail_read { get; set; }


                /// <summary>
                /// Email received date GMT
                /// </summary>
                public string mail_date { get; set; }


                /// <summary>
                /// Email attribute
                /// </summary>
                public object att { get; set; }


                /// <summary>
                /// Email size kb
                /// </summary>
                public string mail_size { get; set; }


                /// <summary>
                /// Email replied to
                /// </summary>
                public string reply_to { get; set; }


                /// <summary>
                /// Content type
                /// Example: text
                /// </summary>
                public string content_type { get; set; }


                /// <summary>
                /// Email recipients
                /// </summary>
                public string mail_recipient { get; set; }


                /// <summary>
                /// Email source id
                /// </summary>
                public int? source_id { get; set; }


                /// <summary>
                /// Email source mail id
                /// </summary>
                public int? source_mail_id { get; set; }


                /// <summary>
                /// Email body
                /// </summary>
                public string mail_body { get; set; }


                /// <summary>
                /// Email size kb
                /// </summary>
                public int? size { get; set; }
            }


            /// <summary>
            /// Email stats class
            /// </summary>
            public class Stats
            {
                public string sequence_mail { get; set; }
                public int created_addresses { get; set; }
                public string received_emails { get; set; }
                public string total { get; set; }
                public string total_per_hour { get; set; }
            }


            /// <summary>
            /// Email auth class
            /// </summary>
            public class Auth
            {
                public bool success { get; set; }
                public List<object> error_codes { get; set; }
            }


            /// <summary>
            /// Main email root class
            /// </summary>
            public class Email
            {
                public List<EmailList> list { get; set; }
                public string count { get; set; }
                public string email { get; set; }
                public string alias { get; set; }
                public int ts { get; set; }
                public string sid_token { get; set; }
                public Stats stats { get; set; }
                public Auth auth { get; set; }
            }


            /// <summary>
            /// Cookie container storage
            /// </summary>
            private CookieContainer mCookies = new CookieContainer();


            /// <summary>
            /// Optional proxy address string
            /// </summary>
            private Proxy mProxy = new Proxy();


            /// <summary>
            /// If we should use the provided proxy
            /// </summary>
            public bool mUseProxy { get; set; }


            /// <summary>
            /// Email address name
            /// </summary>
            private string mEmailAlias;


            /// <summary>
            /// Initializer for the class.
            /// </summary>
            /// <param name="Proxy">Include a Proxy adress string if the request should go via a proxy</param>
            public GuerrillaMail(string proxy = "")
            {
                /*If we got passed a Proxy variable*/
                if (!string.IsNullOrEmpty(proxy))
                {
                    /*Regex to match a proxy*/
                    string ValidIPRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]):[\d]+$";
                    if (Regex.IsMatch(proxy, ValidIPRegex))
                    {
                        string[] pSpl = proxy.Split(':');

                        mProxy.address = pSpl[0];
                        mProxy.port = Convert.ToInt32(pSpl[1]);
                        mProxy.initialized = true;

                        mUseProxy = true;
                    }
                }

                /*Initialize the email*/
                JObject Obj = JObject.Parse(Contact("f=get_email_address"));
                mEmailAlias = ((string)Obj.SelectToken("email_addr")).Split('@')[0];

                /*Delete the automatic welcome email - id is always 1*/
                DeleteSingleEmail("1");
            }


            /// <summary>
            /// Returns full json response
            /// </summary>
            /// <returns></returns>
            public Email GetContent()
            {
                return JsonConvert.DeserializeObject<Email>(Contact("f=get_email_list&offset=0"));
            }


            /// <summary>
            /// Returns all emails in a json string
            /// offset=0 implies getting all emails
            /// </summary>
            /// <returns>Returns list of email</returns>
            public List<EmailList> GetAllEmails()
            {
                var emails = JsonConvert.DeserializeObject<Email>(Contact("f=get_email_list&offset=0"));
                return emails.list;
            }


            /// <summary>
            /// Returns all emails received after a specific email (specified by mail_id)
            /// Example: GetEmailsSinceID(53451833)
            /// </summary>
            /// <param name="mail_id">mail_id of an email</param>
            /// <returns>Returns list of emails</returns>
            public List<EmailList> GetEmailsSinceID(string mail_id)
            {
                var emails = JsonConvert.DeserializeObject<Email>(Contact("f=check_email&seq=" + mail_id));
                return emails.list;
            }


            /// <summary>
            /// Returns the last email
            /// If there are no emails it will return empty string
            /// </summary>
            /// <returns>Returns null if no email</returns>
            public EmailList GetLastEmail()
            {
                var emails = JsonConvert.DeserializeObject<Email>(Contact("f=get_email_list&offset=0"));
                return emails.list.LastOrDefault();
            }


            /// <summary>
            /// Returns our email with a specified domain
            /// </summary>
            /// <param name="domain">Specifies which domain to return (0-8) useful for services that blocks certain domains</param>
            /// <returns>Returns email as string</returns>
            public string GetMyEmail(int domain = 0)
            {
                /*There are several email domains you can use by default*/
                /*Some sites may block guerrilla mail domains, so we can use several different ones*/
                string address = string.Format("{0}@", mEmailAlias);
                switch (domain)
                {
                    case 1:
                        return address + "grr.la";
                    case 2:
                        return address + "guerrillamail.biz";
                    case 3:
                        return address + "guerrillamail.com";
                    case 4:
                        return address + "guerrillamail.de";
                    case 5:
                        return address + "guerrillamail.net";
                    case 6:
                        return address + "guerrillamail.org";
                    case 7:
                        return address + "guerrillamailblock.com";
                    case 8:
                        return address + "spam4.me";

                    default:
                        return address + "sharklasers.com";
                }
            }


            /// <summary>
            /// Deletes an array of emails from the mailbox
            /// </summary>
            /// <param name="mail_ids">String array of mail ids</param>
            public void DeleteEmails(string[] mail_ids)
            {
                /*If there are at least 1 ID in the array*/
                if (mail_ids.Length > 0)
                {
                    /*Go through each array value and format delete string*/
                    string idString = string.Empty;
                    foreach (string id in mail_ids)
                    {
                        /*Example: &email_ids[]53666&email_ids[]53667*/
                        idString += string.Format("&email_ids[]{0}", id);
                    }

                    /*Delete emails*/
                    Contact("f=del_email" + idString);
                }
            }


            /// <summary>
            /// Deletes an array of emails from the mailbox
            /// </summary>
            /// <param name="mail_ids">List string of mail ids</param>
            public void DeleteEmails(List<string> mail_ids)
            {
                /*Format email delete string*/
                string idString = string.Empty;
                mail_ids.ForEach(o => idString += string.Format("&email_ids[]{0}", o));

                /*Delete emails*/
                Contact("f=del_email" + idString);
            }


            /// <summary>
            /// Deletes a single email
            /// </summary>
            /// <param name="mail_id">mail_id of an email</param>
            public void DeleteSingleEmail(string mail_id)
            {
                Contact("f=del_email&email_ids[]=" + mail_id);
            }


            /// <summary>
            /// Calls the page with arguments
            /// </summary>
            /// <param name="parameters">arguments</param>
            /// <returns>Returns json</returns>
            private string Contact(string parameters)
            {
                /*Set up the request*/
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.guerrillamail.com/ajax.php?" + parameters);
                request.CookieContainer = mCookies;
                request.Method = "GET";
                request.Host = "www.guerrillamail.com";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

                /*If we're using a proxy*/
                if (mProxy.initialized && mUseProxy)
                    request.Proxy = new WebProxy(mProxy.address, mProxy.port);

                /*Fetch the response*/
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
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



    /// <summary>
    /// Calls the page with arguments
    /// </summary>
    /// <param name="parameters">arguments</param>
    /// <returns>Returns json</returns>
    private string Contact(string parameters)
        {
            /*Set up the request*/
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.guerrillamail.com/ajax.php?" + parameters);
            request.CookieContainer = mCookies;
            request.Method = "GET";
            request.Host = "www.guerrillamail.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Proxy = new WebProxy(mProxy);

            /*Fetch the response*/
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
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
