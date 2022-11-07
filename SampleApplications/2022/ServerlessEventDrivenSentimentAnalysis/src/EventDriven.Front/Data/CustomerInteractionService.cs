using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using System.Text.Json;

namespace EventDriven.Front.Data
{
    public class CustomerInteractionService
    {
        private readonly AmazonSQSClient sqsClient;
        private readonly AmazonStepFunctionsClient stepFunctionsClient;
        private readonly string queueUrl;

        public CustomerInteractionService(AmazonSQSClient sqsClient, AmazonStepFunctionsClient stepFunctionsClient)
        {
            this.sqsClient = sqsClient;
            this.stepFunctionsClient = stepFunctionsClient;

            this.queueUrl = sqsClient.ListQueuesAsync("AwaitingClaim").Result.QueueUrls[0];
        }

        public async Task<SqsMessage> ClaimCustomerServiceItem()
        {
            var message = await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest()
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 1
            });

            if (message.Messages.Count > 0)
            {
                var customerServiceMessage = JsonSerializer.Deserialize<SqsMessage>(message.Messages[0].Body);
                customerServiceMessage.ReceiptHandle = message.Messages[0].ReceiptHandle;

                return customerServiceMessage;
            }

            return null;
        }

        public async Task SubmitCustomerServiceResponse(SqsMessage customerServiceMessage, string customerServiceAgentName)
        {
            var output = JsonSerializer.Serialize(new ClaimedByTaskResult(customerServiceAgentName));

            await sqsClient.DeleteMessageAsync(queueUrl, customerServiceMessage.ReceiptHandle);

            await stepFunctionsClient.SendTaskSuccessAsync(new SendTaskSuccessRequest()
            {
                TaskToken = customerServiceMessage.TaskToken,
                Output = output
            });
        }
    }
}
