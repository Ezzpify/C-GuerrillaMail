using System;
using Newtonsoft.Json.Linq;

namespace GuerrillaMailExample
{
    class Program
    {
        /* Email json example
        {
            "mail_id":"53312559",
            "mail_from":"somemail@outlook.com",
            "mail_subject":"Activation email",
            "mail_excerpt":"Hey click this link to activate your account http://www.google.com thank you",
            "mail_timestamp":"1441362791",
            "mail_read":"0",
            "mail_date":"9:32:15",
            "att":"0",
            "mail_size":"1050"
        },
        */

        static void DoSomethingWithEmail(string email)
        {
            //Maybe we call a service that requires an email confirmation here
        }

        static void Main(string[] args)
        {
            /*Example one*/
            using (GuerrillaMail mailOne = new GuerrillaMail())
            {
                /*Do something with the email*/
                DoSomethingWithEmail(mailOne.GetMyEmail());

                /*Wait until we have a new email - should probably also wait inbetween the calls*/
                while (mailOne.GetLastEmail() != "")
                {
                    /*Load up the JObject for email*/
                    JObject Email = JObject.Parse(mailOne.GetLastEmail());

                    /*Get the email body as string*/
                    string EmailBody = (string)Email.SelectToken("mail_excerpt");

                    /*Here you can interpret the email or maybe get the activation link from it and navigate to it*/
                    /*Do whatever you want, is what I'm trying to say*/
                    /*When you're done just let the class dispose*/
                }
            }

            /*Example two*/
            /*Works the same way as above, but you can keep it*/
            /*Maybe you could add a bunch of emails to a list and use them for whatever?*/
            /*Endless possibilities...*/
            GuerrillaMail mailTwo = new GuerrillaMail();

            /*Get the different email domains*/
            Console.WriteLine(mailTwo.GetMyEmail(0));
            Console.WriteLine(mailTwo.GetMyEmail(1));
            Console.WriteLine(mailTwo.GetMyEmail(2));

            /*Get all emails*/
            Console.WriteLine(mailTwo.GetAllEmails());
        }
    }
}
