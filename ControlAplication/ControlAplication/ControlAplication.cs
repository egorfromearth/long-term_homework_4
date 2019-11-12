using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using Communication;
using ControlAplication;

namespace ControllAplication
{

    class ControlApplication
    {

        private static List<Task> _tasksList = new List<Task>();  

        static void Main(string[] args)
        {
            if (!MessageQueue.Exists(@".\private$\MainQueue"))
            {
                MessageQueue.Create(@".\private$\MainQueue");
                ProcessingMessage.quereList.Add(@".\private$\MainQueue");
            }

            ProcessingMessage.mainQueue = new MessageQueue(@".\private$\MainQueue");
            ProcessingMessage.mainQueue.Formatter = new BinaryMessageFormatter();


            while (true) {
                ProcessingMessage.ChekMessage(ProcessingMessage.mainQueue);
            }

            

            /*Communication.Message mesage = new Communication.Message(0, ProcessingMessage.agentsList.First().IdAgent);
            MessageQueue quere = new MessageQueue(ProcessingMessage.agentsList.First().Quere);
            quere.Formatter = new BinaryMessageFormatter();

            quere.Send(mesage);



            ExitMessage mesageExit = new ExitMessage(0, ProcessingMessage.agentsList.First().IdAgent);
            quere.Send(mesageExit);

            ProcessingMessage.ChekMessage(ProcessingMessage.mainQueue);*/


            foreach (string index in ProcessingMessage.quereList)
            {
                MessageQueue.Delete(index);
            }
        }
    }
}
