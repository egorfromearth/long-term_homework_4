using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using Communication;

namespace ControllAplication
{

    class ControlApplication
    {

        private static List<Task> _tasksList = new List<Task>();
        private static List<Agent> _agentsList = new List<Agent>();
        private static List<string> _quereList = new List<string>();

        private static Dictionary<string, Delegate> _messageHandler = new Dictionary<string, Delegate>()
        {
            { "HalloMessage",  new HalloMessageHandler(ProcessingHalloMessage) },
            { "ExitMessage",   new ExitMessageHandler(ProcessingExitMessage) },
            { "TaskMessage",  new TaskMessageHandler(ProcessingTaskMessage) }
        };

        private static int _nextIdAgent = 1;
        private static MessageQueue mainQueue;

        delegate bool HalloMessageHandler(MessageQueue quere, HalloMessage message);
        delegate bool ExitMessageHandler(MessageQueue quere, ExitMessage message);
        delegate bool TaskMessageHandler(MessageQueue quere, TaskMessage message);

        public static void DistributionTasks() { }


        /// <summary>
        /// обработчик HalloMessage сообщения
        /// </summary>
        /// <param name="quere">очередь откуда пришло сообщение</param>
        /// <param name="message">приянтое сообщение</param>
        public static bool ProcessingHalloMessage(MessageQueue quere, HalloMessage message)
        {
            try
            {
                Agent agent = new Agent(_nextIdAgent, -1, message.info, @".\private$\secondQuere" + _nextIdAgent.ToString());
                _agentsList.Add(agent);

                MessageQueue.Create(agent.Quere);

                CreateQuereMessage messageSend = new CreateQuereMessage(_nextIdAgent, 0, agent.Quere);
                ++_nextIdAgent;
                quere.Send(messageSend);

                _quereList.Add(agent.Quere);
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// обработчик ExitMessage сообщения
        /// </summary>
        /// <param name="quere">очередь откуда пришло сообщение</param>
        /// <param name="message">приянтое сообщение</param>
        public static bool ProcessingTaskMessage(MessageQueue quere, TaskMessage message)
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// обработчик ExitMessage сообщения
        /// </summary>
        /// <param name="quere">очередь откуда пришло сообщение</param>
        /// <param name="message">приянтое сообщение</param>
        public static bool ProcessingExitMessage(MessageQueue quere, ExitMessage message)
        {
            try
            {
                int idDelete = (message).IdSender;

                foreach (var item in _agentsList)
                {
                    if (item.IdAgent == idDelete)
                    {
                        MessageQueue.Delete(item.Quere);
                        _quereList.Remove(item.Quere);
                        _agentsList.Remove(item);
                        break;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Функция проверяет подключился ли очередной агент к очереди quere и добавляет его в список вгенотв,
        /// так же создается новая очередь сообщений, имя которой записывается в список очередей
        /// </summary>
        /// <param name="quere"></param>
        /// 
        public static void ChekMessage(MessageQueue quere)
        {
            var objMessage = quere.Receive();
            Communication.Message messageRead = (Communication.Message)objMessage.Body;

            foreach (var item in _messageHandler) {
                if (item.Key == messageRead.TypeMessage) {
                    item.Value.DynamicInvoke(quere, messageRead);
                }
            }
        }

        static void Main(string[] args)
        {
            if (!MessageQueue.Exists(@".\private$\MainQueue"))
            {
                MessageQueue.Create(@".\private$\MainQueue");
                _quereList.Add(@".\private$\MainQueue");
            }

            mainQueue = new MessageQueue(@".\private$\MainQueue");
            mainQueue.Formatter = new BinaryMessageFormatter();


            ChekMessage(mainQueue);
            Console.WriteLine(".");

            Communication.Message mesage = new Communication.Message(0, _agentsList.First().IdAgent);
            MessageQueue quere = new MessageQueue(_agentsList.First().Quere);
            quere.Formatter = new BinaryMessageFormatter();

            quere.Send(mesage);



            ExitMessage mesageExit = new ExitMessage(0, _agentsList.First().IdAgent);
            quere.Send(mesageExit);

            ChekMessage(mainQueue);


            foreach (string index in _quereList)
            {
                MessageQueue.Delete(index);
            }
        }
    }
}
