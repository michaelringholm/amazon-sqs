using Amazon;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

namespace om.messaging.queue.aws
{
    public class AmazonSQSQueueService : IQueueService
    {
        private AmazonSQSClient client;
        private AmazonSecurityTokenServiceClient stsClient;

        public AmazonSQSQueueService()
        {
            client = new AmazonSQSClient(RegionEndpoint.EUWest1);
            stsClient = new AmazonSecurityTokenServiceClient();
        }

        public async Task<List<QueueMessage>> GetNextMessageBatch(string queueURL, int maxNumberOfMessages=1)
        {
            var request = new ReceiveMessageRequest
            {
                AttributeNames = new List<string>() { "All" },
                MaxNumberOfMessages = maxNumberOfMessages,
                QueueUrl = queueURL,
                VisibilityTimeout = (int)TimeSpan.FromMinutes(10).TotalSeconds,
                WaitTimeSeconds = (int)TimeSpan.FromSeconds(5).TotalSeconds                
            };

            var response = await client.ReceiveMessageAsync(request);
            Console.WriteLine($"Messages received was [{response.Messages.Count}]");
            if (response.Messages.Count > 0)
            {
                //foreach (var message in response.Messages) PrintMessageDetails(message);
            }
            else Console.WriteLine("No messages received.");
            return response.Messages.Select(m=>QueueMessageAdapter.ToQueueMessage(m)).ToList();
        }

        public async Task PutMessage(string queueURL, object message)
        {
            var response = await client.SendMessageAsync(queueURL, JsonSerializer.Serialize(message));
        }        

        private void PrintMessageDetails(Message message)
        {
            Console.WriteLine("For message ID '" + message.MessageId + "':");
            Console.WriteLine("  Body: " + message.Body);
            Console.WriteLine("  Receipt handle: " + message.ReceiptHandle);
            Console.WriteLine("  MD5 of body: " + message.MD5OfBody);
            Console.WriteLine("  MD5 of message attributes: " + message.MD5OfMessageAttributes);
            Console.WriteLine("  Attributes:");
            foreach (var attr in message.Attributes)
            {
                Console.WriteLine("    " + attr.Key + ": " + attr.Value);
            }
        }

        public async Task<string> GetQueueID(string queueName)
        {
            var getCallerIdentityResponse = await stsClient.GetCallerIdentityAsync(new GetCallerIdentityRequest());
            var accountID = getCallerIdentityResponse.Account;
            Console.WriteLine($"accountID={accountID.Substring(8).PadLeft(12, '*')}");
            var request = new GetQueueUrlRequest
            {
                QueueName = queueName,
                QueueOwnerAWSAccountId = accountID
            };
            var response = await client.GetQueueUrlAsync(request);
            return response.QueueUrl;
        }
    }
}

