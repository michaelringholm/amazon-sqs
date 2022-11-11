namespace om.messaging.queue
{
    public class QueueMessage
    {
        public string MessageID { get; set; }
        public string CorrelationID { get; set; }
        public string Contents { get; set; }
    }
}