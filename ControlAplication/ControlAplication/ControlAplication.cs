using System;
using System.Collections.Generic;
using System.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Communication;
using ControlAplication;

namespace ControllAplication
{

    class ControlApplication
    {
      

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

            Thread threadAgent = new Thread(recervingHalloMessage);
            threadAgent.Start();




            //  Console.Write
            /*
             
            место для раскидывания заданий агентам
             
             */

            while (ProcessingMessage.agentsList.Capacity == 0) ;

            Thread.Sleep(2000);

            var md5 = MD5.Create();
            var tempHash = md5.ComputeHash(Encoding.UTF8.GetBytes("assdw"));
            var hash = Convert.ToBase64String(tempHash);

            Task newTask = new Task();
            newTask.Complete = false;
            newTask.Hash = hash;
            newTask.IdTask = 1;
            newTask.Processing = false;
            newTask.ReadyPassword = "";

            TaskMessage taskMessage = new TaskMessage(0, ProcessingMessage.agentsList[0].IdAgent, newTask, "aa", "bbb");
            ProcessingMessage.agentsList[0].QueueReceive.Send(taskMessage);
                   
            Thread.Sleep(5000);

            foreach (Agent item in ProcessingMessage.agentsList)
            {

               
                ExitMessage exit = new ExitMessage(item.IdAgent, 0);
                item.QueueReceive.Send(exit);
                ProcessingMessage.dictionaryThread[item].Abort();
                ProcessingMessage.dictionaryThread[item].Join();
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
