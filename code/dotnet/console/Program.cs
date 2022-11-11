namespace Amazon.SQS.Demo
{
    using System;
    using Amazon;
    using Microsoft.Extensions.DependencyInjection;
    using om.messaging.queue;
    using om.messaging.queue.aws;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Started!");
            ServiceCollection sc=new ServiceCollection();            
            sc.AddSingleton<IQueueService,AmazonSQSQueueService>();
            
            var queueService=sc.BuildServiceProvider().GetService<IQueueService>();
            RunSQSDemo(queueService).ConfigureAwait(false).GetAwaiter().GetResult();
            Console.WriteLine("Ended!");
        }

        private static async Task RunSQSDemo(IQueueService queueService)
        {            
            var queueID=await queueService.GetQueueID("amazon-sqs-demo");
            await queueService.PutMessage(queueID, new Trade{TradeAmount=600000*new Random().Next(), TradeDate=DateTime.Now, TradeGUID=Guid.NewGuid().ToString()});
            await queueService.GetNextMessageBatch(queueID, 4);
        }
    }
}