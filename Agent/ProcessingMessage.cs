using Communication;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading;

namespace Agent
{
    public class ProcessingMessage
    {
        public static Dictionary<int, Delegate> MessageHandler = new Dictionary<int, Delegate>()
        {
            { (int)EnumTypeMessage.PingMessage ,  new PingMessageHandler(ProcessingPingMessag) },
            { (int)EnumTypeMessage.Message ,  new EmptyMessageHandler(ProcessingMessag) },
            { (int)EnumTypeMessage.ExitMessage,   new ExitMessageHandler(ProcessingExitMessage) },
            { (int)EnumTypeMessage.TaskMessage,  new TaskMessageHandler(ProcessingTaskMessage) },
            { (int)EnumTypeMessage.TaskStopProcessingMessage,  new TaskCompleteMessageHandler(ProcessingTaskStopProcessingMessage) }
        };

        delegate void PingMessageHandler(PingMessage message);
        delegate void EmptyMessageHandler(Communication.Message message);
        delegate void ExitMessageHandler(ExitMessage message);
        delegate void TaskMessageHandler(TaskMessage message);
        delegate void TaskCompleteMessageHandler(TaskStopProcessingMessage message);

        public static void ProcessingPingMessag(PingMessage message)
        {
            PingMessage pingMessage = new PingMessage(0, Agent.IdAgent);
            Agent.QueueSend.Send(pingMessage);
        }

        public static void ProcessingMessag(Communication.Message message)
        {
            Console.WriteLine(message.TypeMessage + "Agent idSender:" + message.IdSender);
        }

        public static void ProcessingTaskStopProcessingMessage(TaskStopProcessingMessage message) {
      
            if (Agent.threadDictionary.ContainsKey(message.IdTask))
            {
                foreach (var item in Agent.threadDictionary[message.IdTask]) {
                    item.Abort();
                }

                for (int index = 0; index < Agent.tasksList.Count; ++index)
                {
                    Agent.tasksList.Remove(Agent.tasksList[index]);
                }
            }
        }

        public static void ProcessingExitMessage(ExitMessage message)
        {
            Console.WriteLine(message.TypeMessage);
            ExitMessage sendMessage = new ExitMessage(0, Agent.IdAgent);
            try
            {
                Agent.QueueSend.Send(sendMessage);

                Agent.QueueReceive.Close();
                Agent.QueueSend.Close();
                Agent.mainQuere.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            Environment.Exit(0);
        }

        public static void newThreadBruteForce(TaskMessage message)
        {

            PasswordGuessing passwordGuessing = new PasswordGuessing();
            passwordGuessing.Brute(message.Start, message.Stop, message.Task.Hash);

            if (PasswordGuessing.pass == "")
            {
                TaskMessage messageSend = new TaskMessage(0, Agent.IdAgent, message.Task, message.Start, message.Stop);
                Agent.QueueSend.Send(messageSend);
            }
            else
            {
                Task compliteTask = new Task(message.Task.Hash, message.Task.IdTask);
                compliteTask = message.Task;
                compliteTask.ReadyPassword = PasswordGuessing.pass;
                compliteTask.Complete = true;

                TaskMessage messageSend = new TaskMessage(0, Agent.IdAgent, compliteTask, message.Start, message.Stop);
                Agent.QueueSend.Send(messageSend);
            }
        }

        public static void ProcessingTaskMessage(TaskMessage message)
        {
            Console.WriteLine("segment: " + message.Start + " - " + message.Stop + " Hash: " + message.Task.Hash);
            Agent.tasksList.Add(message.Task);

            Thread myThread = new Thread(delegate () { newThreadBruteForce(message); });
            myThread.Start();

            if (Agent.threadDictionary.ContainsKey(message.Task.IdTask))
            {
                Agent.threadDictionary[message.Task.IdTask].Add(myThread);
            }
            else {
                List<Thread> list = new List<Thread>() { myThread };
                Agent.threadDictionary.Add(message.Task.IdTask, list);
            }
        }

        public static int ChekMessage(MessageQueue quere)
        {
            try
            {
                var objMessage = quere.Receive();
                Communication.Message messageRead = (Communication.Message)objMessage.Body;

                foreach (var item in MessageHandler)
                {
                    if (item.Key == messageRead.TypeMessage)
                    {
                        item.Value.DynamicInvoke(messageRead);
                        return item.Key;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return -1;
        }

        /// <summary>
        /// Функция ждет когда управляющее приложение отправит сообщение с именем очереди 
        /// и присвоенным id агенту, подключается к этой очереди
        /// </summary>
        /// <returns>
        /// true если подключиться получилось, false если иначе
        /// </returns>
        public static bool Connecting(string nameQueue)
        {
            Console.WriteLine("Ожидание связи с управляющим приложнием...");
            do
            {
                Agent.mainQuere = new MessageQueue(nameQueue);
                Thread.Sleep(1000);
            } while (!Agent.mainQuere.CanRead);

            Console.WriteLine("Соединение установлено!");

            HalloMessage messagSend = new HalloMessage(0, Agent.IdAgent, Agent.Info);

            try
            {
                Agent.mainQuere.Formatter = new BinaryMessageFormatter();
                Agent.mainQuere.Send(messagSend);

                var objMessage = Agent.mainQuere.Receive();
                while (((Communication.Message)objMessage.Body).TypeMessage != (int)EnumTypeMessage.CreateQuereMessage)
                {
                    Communication.Message tempMessage = new Communication.Message(0, Agent.IdAgent);
                    tempMessage = (Communication.Message)objMessage.Body;
                    Agent.mainQuere.Send(tempMessage);

                    Thread.Sleep(1000);
                    objMessage = Agent.mainQuere.Receive();
                }

                CreateQuereMessage messageRead = (CreateQuereMessage)objMessage.Body;

                Agent.IdAgent = messageRead.IdRecepient;
                Agent.NameQueueReceive = messageRead.PathReceive;
                Agent.NameQueueSend = messageRead.PathSend;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            Agent.QueueReceive = new MessageQueue("FormatName:DIRECT=OS:" + Agent.NameQueueReceive);
            Agent.QueueReceive.Formatter = new BinaryMessageFormatter();

            Agent.QueueSend = new MessageQueue("FormatName:DIRECT=OS:" + Agent.NameQueueSend);
            Agent.QueueSend.Formatter = new BinaryMessageFormatter();

            Agent.QueueSend.Send(messagSend);

            return true;
        }
    }
}