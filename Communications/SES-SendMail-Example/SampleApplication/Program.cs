using System;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace ses_sendmail_example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sending Email...");
            using (var client = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.USEast1))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = "verified-email-address@example.com",
                    Destination = new Destination { ToAddresses = {"dest@example.com"} },
                    Message = new Message
                    {
                        Subject = new Content("Hello from the Amazon Simple Email Service!"),
                        Body = new Body
                        {
                            Html = new Content("<html><body><h2>Hello from Amazon SES</h2><ul><li>I'm a list item</li><li>So am I!</li></body></html>")
                        }
                    }
                };

                try
                {
                    var response = client.SendEmailAsync(sendRequest).Result;
                    Console.WriteLine("Email sent! Message ID = {0}", response.MessageId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Send failed with exception: {0}", ex.Message);
                }
            }

            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
