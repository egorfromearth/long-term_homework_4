using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading;
using Communication;

namespace ControlApplication
{    
    public class ControlApplication
    {
        private static int _nextIdTask = 1;

        public static Agent GetFreeAgent(List<Agent> agentsList) {

            foreach (var item in agentsList) {
                if (item.Info.CountCore > 0 && item.Connect) {
                    return item;
                }
            }
            return null;
        }

        public static void recervingHalloMessage()
        {
            while (true)
            {
                ProcessingMessage.ChekMessage(ProcessingMessage.mainQueue);
            }
        }

        static void Main(string[] args)
        {
            if (!MessageQueue.Exists(@".\private$\MainQueue"))
            {
                MessageQueue.Create(@".\private$\MainQueue");
            }            

            ProcessingMessage.mainQueue = new MessageQueue(@".\private$\MainQueue");
            ProcessingMessage.mainQueue.Formatter = new BinaryMessageFormatter();

            ProcessingMessage.mainQueue.SetPermissions("АНОНИМНЫЙ ВХОД", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);

            Thread threadAgent = new Thread(recervingHalloMessage);
            threadAgent.Start();
            string read = "";
            int _countHash = 0;

            do
            {
                Console.Write("1. Добавить задание\n");
                Console.Write("2. Посмотреть список заданий\n");
                Console.Write("3. Выйти\n");

                read = Console.ReadLine();
                if (read == "1")
                {

                    if (ProcessingMessage.agentsList.Count != 0)
                    {
                        Console.WriteLine("Введите колличество сверток");

                        try
                        {
                            _countHash = Convert.ToInt32(Console.ReadLine());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            continue;
                        }

                        Console.WriteLine("Введите все свертки по одной");

                        for (int index = 0; index < _countHash; ++index)
                        {
                            Task task = new Task(Console.ReadLine(), _nextIdTask);
                            ++_nextIdTask;

                            ProcessingMessage.tasksList.Add(task);
                        }

                        for (int index = 0; index < ProcessingMessage.tasksList.Count; ++index)
                        {
                            if (!ProcessingMessage.tasksList[index].Processing)
                            {
                                ProcessingMessage.tasksList[index].Processing = true;
                                var messageArr = DistributionRanges.Distribution(ProcessingMessage.agentsList, ProcessingMessage.tasksList[index]);
                                ProcessingMessage.SendMessageArray(messageArr, ProcessingMessage.agentsList);

                            }
                        }
                    }
                    else {
                        Console.WriteLine("Нет ни одного подключенного агента.");
                    }
                }
                else if(read == "2") {
                    foreach (var item in ProcessingMessage.tasksList) {
                        Console.WriteLine(" Hash: " + item.Hash + "\n Complete: " + item.Complete + "\n ReadyPassword: " + item.ReadyPassword + '\n');
                    }
                }

            } while (read != "3");
      
            foreach (Agent item in ProcessingMessage.agentsList)
            {        
                ExitMessage exit = new ExitMessage(item.IdAgent, 0);
                item.QueueReceive.Send(exit);
                ProcessingMessage.dictionaryThread[item][0].Abort();
                ProcessingMessage.dictionaryThread[item][1].Abort();
            }

            foreach (string item in ProcessingMessage.quereList)
            {
                MessageQueue.Delete(item);
            }

            threadAgent.Abort();
            Environment.Exit(0);
        }
    }
}