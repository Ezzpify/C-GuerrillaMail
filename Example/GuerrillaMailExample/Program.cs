using System;
using Newtonsoft.Json.Linq;

namespace GuerrillaMailExample
{
    class Program
    {
        /// <summary>
        /// Global email
        /// </summary>
        static GuerrillaMail mailThree;


        /// <summary>
        /// Here we're doing something with the email
        /// </summary>
        /// <param name="email"></param>
        static void DoSomethingWithEmail(string email)
        {
            //Maybe we call a service that requires an email confirmation here
        }


        /// <summary>
        /// Here we'll do something entierly different
        /// </summary>
        static void DoSomethingElseThatTakesTime()
        {
            //Wow that's a lot of code that's doing something else
        }


        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">No args</param>
        static void Main(string[] args)
        {
            /*
                Example One - Using IDisposable
                We'll get the email body here then let it dispose
            */
            using (GuerrillaMail mailOne = new GuerrillaMail())
            {
                /*Get our default email address and do something with it*/
                var myEmailAddress = mailOne.GetMyEmail();
                DoSomethingWithEmail(myEmailAddress);

                /*Get last email and print the text content (body)*/
                var lastEmail = mailOne.GetLastEmail();
                Console.WriteLine(lastEmail.mail_excerpt);
            }


            /*
                Example Two - Doing it local
                Print our different emails
            */
            GuerrillaMail mailTwo = new GuerrillaMail();
            Console.WriteLine(mailTwo.GetMyEmail(1));
            Console.WriteLine(mailTwo.GetMyEmail(2));
            Console.WriteLine(mailTwo.GetMyEmail(3));


            /*
                Example Three - We're going global
                This time we'll make the mail global so we can use it later
            */
            mailThree = new GuerrillaMail();
            DoSomethingElseThatTakesTime();

            /*Oops now we need to get ALL the email we've received*/
            DoSomethingWithEmail(mailThree.GetMyEmail());
            var myEmails = mailThree.GetAllEmails();
        }
    }
}
