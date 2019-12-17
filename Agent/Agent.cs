using Communication;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading;

namespace Agent
{
    public class Agent
    {
        public static int IdAgent = -1;
        public static List<Task> tasksList = new List<Task>();
        public static Dictionary<int, List<Thread>> threadDictionary = new Dictionary<int, List<Thread>>();
        public static SystemInformation Info = new SystemInformation();
        public static MessageQueue mainQuere;

        public static string NameQueueSend { get; set; }
        public static string NameQueueReceive { get; set; }
        public static MessageQueue QueueSend { get; set; }
        public static MessageQueue QueueReceive { get; set; }


        static void Main(string[] args)
        {
            PasswordGuessing temp = new PasswordGuessing();
            Info.CountCore = Environment.ProcessorCount;
            Info.PasswordsPerSecond = temp.SpeedTest();

            if (!ProcessingMessage.Connecting(@"FormatName:DIRECT=OS:winserver\private$\mainqueue"))
            {
                Environment.Exit(1);
            }

            while (true)
            {
                ProcessingMessage.ChekMessage(QueueReceive);
            }
        }
    }
}
