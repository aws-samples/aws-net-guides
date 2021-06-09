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

            /* The code in this sample assumes you have a credential profile names 'default',
             * which the SDK will use unless overridden.
             * To use credentials from a different profile, uncomment the code below
             * and change the instantiation of the AmazonSimpleEmailService client as shown.
            */

            // code to use a specific credential profile
            // var chain = new Amazon.Runtime.CredentialManagement.CredentialProfileStoreChain();
            // Amazon.Runtime.AWSCredentials awsCredentials;
            // if (!chain.TryGetAWSCredentials("steve-demo", out awsCredentials))
            // {
            //     throw new Exception("Unable to load credentials from the specified profile");
            // }
            // using (var client = new AmazonSimpleEmailServiceClient(awsCredentials, Amazon.RegionEndpoint.USEast1))

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
