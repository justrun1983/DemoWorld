using System.Net;
using System.Net.Mail;
using SendGrid;


namespace SendGridDemo
{
    class Program
    {
        private static void Main(string[] args)
        {
            // Create the email object first, then add the properties.
            var myMessage = new SendGridMessage();
            myMessage.AddTo("test@test.com");
            myMessage.From = new MailAddress("john@example.com", "John Smith");
            myMessage.Subject = "Testing the SendGrid Library";
            myMessage.Text = "Hello World!";

            // Create credentials, specifying your user name and password.
            var credentials = new NetworkCredential("username", "password");

            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            transportWeb.Deliver(myMessage);
        }
    }
}
