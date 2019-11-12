using Communication;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading;

namespace Agent
{
    class ProcessingMessage
    {
        public static int _idAgent = -1;
        public static string _nameQueueSend;
        public static string _nameQueueReceive;
        public static MessageQueue _queueSend;
        public static MessageQueue _queueReceive;
        public static List<Communication.Task> _tasks = new List<Communication.Task>();
        public static SystemInformation _info = new SystemInformation();
        public static MessageQueue _mainQuere = new MessageQueue(@".\private$\MainQueue");

        public static Dictionary<string, Delegate> _messageHandler = new Dictionary<string, Delegate>()
        {
            { "Message",  new MessageHandler(ProcessingMessag) },
            { "ExitMessage",   new ExitMessageHandler(ProcessingExitMessage) },
            { "TaskMessage",  new TaskMessageHandler(ProcessingTaskMessage) }
        };

        delegate void MessageHandler(Communication.Message message);
        delegate void ExitMessageHandler(ExitMessage message);
        delegate void TaskMessageHandler(TaskMessage message);

        public static void ProcessingMessag(Communication.Message message)
        {
            Console.WriteLine(message.TypeMessage + "Agent idSender:" + message.IdSender);
        }

        public static void ProcessingExitMessage(ExitMessage message)
        {
            Console.WriteLine(message.TypeMessage);
            ExitMessage sendMessage = new ExitMessage(0, _idAgent);

            _mainQuere.Send(sendMessage);
            _queueReceive.Close();
            _queueSend.Close();
            _mainQuere.Close();

            Environment.Exit(0);
        }

        public static void ProcessingTaskMessage(TaskMessage message)
        {
            Console.WriteLine(message.TypeMessage);
            _tasks.Add(message.Task);

        }

        /// <summary>
        /// Функция ждет когда управляющее приложение отправит сообщение с именем очереди 
        /// и присвоенным id агенту, подключается к этой очереди
        /// </summary>
        /// <returns>
        /// true если подключиться получилось, false если иначе
        /// </returns>
        public static bool Connecting()
        {
            while (!MessageQueue.Exists(@".\private$\MainQueue"))
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Ожидание связи с управляющим приложнием");
            }

            try
            {

                _mainQuere.Formatter = new BinaryMessageFormatter();

                HalloMessage messagSend = new HalloMessage(0, _idAgent, _info);
                _mainQuere.Send(messagSend);

                var objMessage = _mainQuere.Receive();
                while (((Communication.Message)objMessage.Body).TypeMessage != "CreateQuereMessage")
                {
                    _mainQuere.Send(messagSend);
                    Thread.Sleep(5000);
                    objMessage = _mainQuere.Receive();
                }

                CreateQuereMessage messageRead = (CreateQuereMessage)objMessage.Body;

                _idAgent = messageRead.IdRecepient;
                _nameQueueReceive = messageRead.PathReceive;
                _nameQueueSend = messageRead.PathSend;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            _queueReceive = new MessageQueue(_nameQueueReceive);
            _queueReceive.Formatter = new BinaryMessageFormatter();

            _queueSend = new MessageQueue(_nameQueueSend);
            _queueSend.Formatter = new BinaryMessageFormatter();

            return true;
        }
    }
}
