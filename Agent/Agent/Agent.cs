using System;
using System.Collections.Generic;
using System.Messaging;
using Communication;
using System.Threading;

namespace Agent
{

    class Agent
    {
        private static int _idAgent = -1;
        private static string _path = null;
        private static MessageQueue _quere;
        private static MessageQueue _mainQuere = new MessageQueue(@".\private$\MainQueue");
        private static List<Task> _tasks = new List<Task>();
        private static SystemInformation _info = new SystemInformation();

        private static Dictionary<string, Delegate> _messageHandler = new Dictionary<string, Delegate>()
        {
            { "Message",  new MessageHandler(ProcessingMessage) },
            { "ExitMessage",   new ExitMessageHandler(ProcessingExitMessage) },
            { "TaskMessage",  new TaskMessageHandler(ProcessingTaskMessage) }
        };

        delegate void MessageHandler(Communication.Message message);
        delegate void ExitMessageHandler(ExitMessage message);
        delegate void TaskMessageHandler(TaskMessage message);

        public static void ProcessingMessage(Communication.Message message) {
            Console.WriteLine(message.TypeMessage);
        }

        public static void ProcessingExitMessage(ExitMessage message)
        {
            Console.WriteLine(message.TypeMessage);
            ExitMessage sendMessage = new ExitMessage(0, _idAgent);

            _mainQuere.Send(sendMessage);
            _quere.Close();
            _mainQuere.Close();
            Environment.Exit(0);
                     
        }

        public static void ProcessingTaskMessage(TaskMessage message)
        {
            Console.WriteLine(message.TypeMessage);
            _tasks.Add(message.Task);

            Thread brute = new Thread(bruteForce);
            brute.Start(message.Task.IdTask);
        }

        private static void SetInfo() {
            _info.CountCore = Environment.ProcessorCount;
            _info.PasswordsPerSecond = 555; //надо как то узнать скорость подбора
        }

        /// <summary>
        /// Функция ждет когда управляющее приложение отправит сообщение с именем очереди 
        /// и присвоенным id агенту, подключается к этой очереди
        /// </summary>
        /// <returns>
        /// true если подключиться получилось, false если иначе
        /// </returns>
        private static bool Connecting() {
            while (!MessageQueue.Exists(@".\private$\MainQueue"))
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Ожидание связи с управляющим приложнием");
            }
               
            try { 
                             
                _mainQuere.Formatter = new BinaryMessageFormatter();

                HalloMessage messagSend = new HalloMessage(0, _idAgent, _info);
                _mainQuere.Send(messagSend);

                var objMessage = _mainQuere.Receive();

                CreateQuereMessage messageRead = (CreateQuereMessage) objMessage.Body;

                _idAgent = messageRead.IdRecepient;
                _path = messageRead.Path;
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            _quere = new MessageQueue(_path);
            _quere.Formatter = new BinaryMessageFormatter();

            return true;
        }

        public static void bruteForce(Object input) {
            int idTask = (int)input;
            
            //с этим индексом работать при подборе пароля
            int indexTaskinList = _tasks.FindIndex((item) => item.IdTask == idTask);

            
            TaskMessage taskMessage = new TaskMessage(_idAgent, 0, _tasks[indexTaskinList]);
            _mainQuere.Send(taskMessage);
        }

        static void Main(string[] args)
        {
            SetInfo();
            if (!Connecting()) {
                Environment.Exit(1);
            }

            while (true) {

                var messageRead = _quere.Receive();
                Communication.Message message = (Communication.Message)messageRead.Body;

                foreach (var item in _messageHandler)
                {
                    if (item.Key == message.TypeMessage)
                    {
                        item.Value.DynamicInvoke(message);
                    }
                }
            }                   
        }
    }
}
