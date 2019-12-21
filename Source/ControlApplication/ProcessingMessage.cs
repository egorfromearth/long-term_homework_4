using System;
using System.Collections.Generic;
using System.Messaging;
using Communication;
using System.Threading;

namespace ControlApplication
{

    public class ProcessingMessage
    {
        public static List<Task> tasksList = new List<Task>();
        public static List<Agent> agentsList = new List<Agent>();
        public static List<string> quereList = new List<string>();
        public static Dictionary<Agent, Thread[]> dictionaryThread = new Dictionary<Agent, Thread[]>();

        public static DateTime Time = DateTime.Now;
        public static MessageQueue mainQueue;
        private static int _nextIdAgent = 1;

        delegate void PingMessageHandler(MessageQueue quere, PingMessage message);
        delegate bool HalloMessageHandler(MessageQueue quere, HalloMessage message);
        delegate bool ExitMessageHandler(MessageQueue quere, ExitMessage message);
        delegate bool TaskMessageHandler(MessageQueue quere, TaskMessage message);
        delegate bool EmptyMessageHandler(MessageQueue quere, Communication.Message message);

        private static Dictionary<int, Delegate> MessageHandler = new Dictionary<int, Delegate>()
        {
            { (int)EnumTypeMessage.PingMessage ,  new PingMessageHandler(ProcessingPingMessag) },
            { (int)EnumTypeMessage.Message,  new EmptyMessageHandler(ProcessingMessag) },
            { (int)EnumTypeMessage.HelloMessage,  new HalloMessageHandler(ProcessingHelloMessage) },
            { (int)EnumTypeMessage.ExitMessage,   new ExitMessageHandler(ProcessingExitMessage) },
            { (int)EnumTypeMessage.TaskMessage,  new TaskMessageHandler(ProcessingTaskMessage) }
        };


        /// <summary>
        /// обработчик PingMessag сообщеия 
        /// </summary>
        /// <param name="quere">очередь откуда пришло сообщение</param>
        /// <param name="message">приянтое сообщение</param>
        public static void ProcessingPingMessag(MessageQueue quere, PingMessage message)
        {
            for (int index = 0; index < agentsList.Count; ++index)
            {
                if (agentsList[index].IdAgent == message.IdSender)
                {
                    agentsList[index].LastConnect = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Производит рассылку сообщений агентам из списка agentList (сообщение отправляется агенту, id которго указан данном сообщении)
        /// </summary>
        /// <param name="messageArr"></param>
        /// <param name="agentsList"></param>
        /// <returns></returns>
        public static bool SendMessageArray(Communication.Message[] messageArr, List<Agent> agentsList)
        {
            try
            {
                foreach (var itemMessage in messageArr)
                {
                    foreach (var itemAgent in agentsList)
                    {
                        if (itemAgent.IdAgent == itemMessage.IdRecepient)
                        {
                            itemAgent.QueueReceive.Send(itemMessage);
                        }
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
        /// обработчик пустого сообщения
        /// </summary>
        /// <param name="quere">очередь откуда пришло сообщение</param>
        /// <param name="message">приянтое сообщение</param>
        public static bool ProcessingMessag(MessageQueue quere, Communication.Message message)
        {
            try
            {
                Console.WriteLine("ControllAplication idSender:" + message.IdSender + " idIdRecepient:" + message.IdRecepient);
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
        public static bool ProcessingHelloMessage(MessageQueue quere, HalloMessage message)
        {
            try
            {
                Agent agent = new Agent(_nextIdAgent, message.Info, Environment.MachineName+@"\private$\secondSend" + _nextIdAgent.ToString(), Environment.MachineName + @"\private$\secondRecive" + _nextIdAgent.ToString());

                if (!MessageQueue.Exists(agent.QueueSendName))
                {
                    MessageQueue.Create(agent.QueueSendName);
                }

                if (!MessageQueue.Exists(agent.QueueReceiveName))
                {
                    MessageQueue.Create(agent.QueueReceiveName);
                }
                agent.SetConnect();

                agent.QueueReceive.SetPermissions("АНОНИМНЫЙ ВХОД", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
                agent.QueueSend.SetPermissions("АНОНИМНЫЙ ВХОД", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);

                CreateQuereMessage messageSend = new CreateQuereMessage(_nextIdAgent, 0, agent.QueueSendName, agent.QueueReceiveName);
                ++_nextIdAgent;
                quere.Send(messageSend);

                quereList.Add(agent.QueueSendName);
                quereList.Add(agent.QueueReceiveName);

                agent.QueueSend.Receive();
                agentsList.Add(agent);

                Thread threadWorkingAgent = new Thread(agent.WorkingAgent);
                Thread threadPingAgent = new Thread(agent.PingAgent);
                threadWorkingAgent.Start();
                threadPingAgent.Start();

                Thread[] arrayThread = new Thread[2] { threadWorkingAgent, threadPingAgent };

                dictionaryThread.Add(agent, arrayThread);
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
                Console.Write("TaskMessage idAgent: " + message.IdSender);
                for (int index = 0; index < tasksList.Count; ++index)
                {
                    if (tasksList[index].IdTask == message.Task.IdTask && message.Task.Complete)
                    {
                        tasksList[index] = message.Task;
                    }
                }
                if (message.Task.Complete)
                {
                    TaskStopProcessingMessage taskCompleteMessage = new TaskStopProcessingMessage(message.IdSender, 0, message.Task.IdTask);

                    for (int index = 0; index < agentsList.Count; ++index) {
                        foreach (var itemTask in agentsList[index].Tasks) {
                            if (itemTask.Key.IdTask == message.Task.IdTask) {
                                agentsList[index].Info.CountCore++;
                                agentsList[index].QueueReceive.Send(taskCompleteMessage);
                            }
                        }
                    } 

                    Console.WriteLine("пароль к хешу " + message.Task.Hash +" подобран: " + message.Task.ReadyPassword);
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
                        MessageQueue.Delete(item.QueueReceiveName);
                        MessageQueue.Delete(item.QueueSendName);
                        quereList.Remove(item.QueueReceiveName);
                        quereList.Remove(item.QueueSendName);
                        agentsList.Remove(item);
                        dictionaryThread[item][0].Abort();
                        dictionaryThread[item][1].Abort();
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
        public static int ChekMessage(MessageQueue quere)
        {
            var objMessage = quere.Receive();
            Communication.Message messageRead = (Communication.Message)objMessage.Body;

            foreach (var item in MessageHandler)
            {
                if (item.Key == messageRead.TypeMessage)
                {
                    item.Value.DynamicInvoke(quere, messageRead);
                    return item.Key;
                }
            }
            return -1;
        } 
    }
}
