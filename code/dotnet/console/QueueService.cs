using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SQS.Model;

namespace Amazon.SQS.Demo
{
    public class QueueService
    {
        private AmazonSQSClient client;
        private AmazonSecurityTokenServiceClient stsClient;

        public QueueService()
        {
            client = new AmazonSQSClient(RegionEndpoint.EUWest1);
            stsClient = new AmazonSecurityTokenServiceClient();            
        }

        public async Task<List<Message>> GetNextMessage(string queueURL)
        {            
            var request = new ReceiveMessageRequest
            {
                AttributeNames = new List<string>() { "All" },
                MaxNumberOfMessages = 5,
                QueueUrl = queueURL,                
                VisibilityTimeout = (int)TimeSpan.FromMinutes(10).TotalSeconds,
                WaitTimeSeconds = (int)TimeSpan.FromSeconds(5).TotalSeconds
            };

            var response = await client.ReceiveMessageAsync(request);
            if (response.Messages.Count > 0)
            {
                foreach (var message in response.Messages)
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
            }
            else
            {
                Console.WriteLine("No messages received.");
            }
            return response.Messages;
        }

        public async Task<string> GetQueueURL(string queueName)
        {            
            var getCallerIdentityResponse = await stsClient.GetCallerIdentityAsync(new GetCallerIdentityRequest());
            var accountID=getCallerIdentityResponse.Account;
            Console.WriteLine($"accountID={accountID.Substring(8).PadLeft(12,'*')}");
            var request = new GetQueueUrlRequest
            {
                QueueName = queueName,
                QueueOwnerAWSAccountId = accountID
            };
            var response=await client.GetQueueUrlAsync(request);
            return response.QueueUrl;
        }
    }
}
