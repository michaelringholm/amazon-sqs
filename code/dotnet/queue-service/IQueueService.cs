namespace om.messaging.queue;
public interface IQueueService
{
    Task<List<QueueMessage>> GetNextMessageBatch(string queueURL, int maxNumberOfMessages=1);
    Task PutMessage(string queueID, object message);
    Task<string> GetQueueID(string queueName);
}
