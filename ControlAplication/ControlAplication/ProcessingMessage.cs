using System;
using System.Collections.Generic;
using System.Messaging;
using Communication;
using System.Threading;
using ControllAplication;

namespace ControlAplication
{

    public class ProcessingMessage
    {
        public static List<Agent> agentsList = new List<Agent>();
        public static List<string> quereList = new List<string>();
        public static Dictionary<Agent, Thread> dictionaryThread = new Dictionary<Agent, Thread>();

        public static MessageQueue mainQueue;
        private static int _nextIdAgent = 1;

        delegate bool HalloMessageHandler(MessageQueue quere, HalloMessage message);
        delegate bool ExitMessageHandler(MessageQueue quere, ExitMessage message);
        delegate bool TaskMessageHandler(MessageQueue quere, TaskMessage message);
        delegate bool MessageHandler(MessageQueue quere, Communication.Message message);

        private static Dictionary<string, Delegate> _messageHandler = new Dictionary<string, Delegate>()
        {
            { "Message",  new MessageHandler(ProcessingMessag) },
            { "HalloMessage",  new HalloMessageHandler(ProcessingHalloMessage) },
            { "ExitMessage",   new ExitMessageHandler(ProcessingExitMessage) },
            { "TaskMessage",  new TaskMessageHandler(ProcessingTaskMessage) }
        };

        public static void DistributionTasks() { }

        public static bool ProcessingMessag(MessageQueue quere, Communication.Message message)
        {
            try
            {
                Console.WriteLine("ControllAplication idSender:"+ message.IdSender + " idIdRecepient:" + message.IdRecepient);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// обработчик HalloMessage сообщения
        /// </summary>
        /// <param name="quere">очередь откуда пришло сообщение</param>
        /// <param name="message">приянтое сообщение</param>
        public static bool ProcessingHalloMessage(MessageQueue quere, HalloMessage message) 
        {
            try
            {
                Agent agent = new Agent(_nextIdAgent, -1, message.info, @".\private$\secondSend" + _nextIdAgent.ToString(), @".\private$\secondRecive" + _nextIdAgent.ToString());
                agentsList.Add(agent);

                if (!MessageQueue.Exists(agent.QueueSend))
                {
                    MessageQueue.Create(agent.QueueSend);
                }

                if (!MessageQueue.Exists(agent.QueueReceive))
                {
                    MessageQueue.Create(agent.QueueReceive);
                }

                CreateQuereMessage messageSend = new CreateQuereMessage(_nextIdAgent, 0, agent.QueueSend, agent.QueueReceive);
                ++_nextIdAgent;
                quere.Send(messageSend);

                quereList.Add(agent.QueueSend);
                quereList.Add(agent.QueueReceive);

                Thread threadAgent = new Thread(agent.WorkingAgent);
                threadAgent.Start();

                dictionaryThread.Add(agent, threadAgent);
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

                foreach (var item in agentsList)
                {
                    if (item.IdAgent == idDelete)
                    {
                        MessageQueue.Delete(item.QueueReceive);
                        MessageQueue.Delete(item.QueueSend);
                        quereList.Remove(item.QueueReceive);
                        quereList.Remove(item.QueueSend);
                        agentsList.Remove(item);
                        dictionaryThread[item].Abort();
                        dictionaryThread.Remove(item);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Функция проверяет подключился ли очередной агент к очереди quere и добавляет его в список aгентов,
        /// так же создается новая очередь сообщений, имя которой записывается в список очередей
        /// для работы с подключенным агентом создается новый поток
        /// </summary>
        /// <param name="quere">
        /// имя просматриваемй очереди
        /// </param>
        /// 
        public static void ChekMessage(MessageQueue quere)
        {
            var objMessage = quere.Receive();
            Communication.Message messageRead = (Communication.Message)objMessage.Body;

            foreach (var item in _messageHandler)
            {
                if (item.Key == messageRead.TypeMessage)
                {
                    item.Value.DynamicInvoke(quere, messageRead);
                }
            }
        }
    }
}
