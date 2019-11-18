using Communication;
using ControlAplication;
using System.Messaging;

namespace ControllAplication
{
    public class Agent
    {
        int _idAgent;
        int _idTask;
        string _nameQueueSend;
        string _nameQueueReceive;
        MessageQueue _queueSend;
        MessageQueue _queueReceive;
        SystemInformation _info;

        public int IdAgent { get => _idAgent; }
        public int IdTaskt { get => _idTask; }
        public string QueueSendName { get => _nameQueueSend; }
        public string QueueReceiveName { get => _nameQueueReceive; }
        public MessageQueue QueueSend { get => _queueSend; }
        public MessageQueue QueueReceive { get => _queueReceive; }

        SystemInformation Info { get => _info; }

        public void WorkingAgent()
        {

            while (true)
            {
                ProcessingMessage.ChekMessage(_queueSend);
            }
        }

        public Agent(int idAgent, int idTask, SystemInformation info, string queueSend, string queueReceive)
        {
            _idAgent = idAgent;
            _idTask = idTask;
            _nameQueueSend = queueSend;
            _nameQueueReceive = queueReceive;
            _info = info;

            _queueSend = new MessageQueue(_nameQueueSend);
            _queueSend.Formatter = new BinaryMessageFormatter();

            _queueReceive = new MessageQueue(_nameQueueReceive);
            _queueReceive.Formatter = new BinaryMessageFormatter();
        }
    }
}
