using Amazon.SQS.Model;

namespace om.messaging.queue.aws
{
    internal class QueueMessageAdapter
    {
        public static QueueMessage ToQueueMessage(Message amazonSQSMessage)
        {
            var queueMessage=new QueueMessage();
            return queueMessage;
        }
    }
}