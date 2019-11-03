using System;
using System.Collections.Generic;
using System.Messaging;

namespace Agent
{
       
    class Agent
    {
        private int _idAgent;
        private static Task _tasks; //Id/hash
        private SystemInformation _info;
        

        static void Main(string[] args)
        {
            if (!MessageQueue.Exists(".\\$private\\MainQueue"))
            {
                Console.WriteLine("Нет связи с управляющим приложением");
                Environment.Exit(0);
            }

            MessageQueue queue = new MessageQueue(".\\private$\\MainQueue");
            queue.Formatter = new BinaryMessageFormatter();

            Message messag = new Message(0, -1);
            queue.Send(messag);


        }
    }
}



