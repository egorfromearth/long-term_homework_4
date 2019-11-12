using System;
using System.Threading;

namespace Agent
{

    class Agent
    {
              
        private static void SetInfo() {
            ProcessingMessage._info.CountCore = Environment.ProcessorCount;
            ProcessingMessage._info.PasswordsPerSecond = 555; //надо как то узнать скорость подбора
        }

        static void Main(string[] args)
        {
            SetInfo();
            if (!ProcessingMessage.Connecting()) {
                Environment.Exit(1);
            }

            while (true) {

                Communication.Message message = new Communication.Message(0, ProcessingMessage._idAgent);

                ProcessingMessage._queueSend.Send(message);
                Thread.Sleep(2000);

                var messageRead = ProcessingMessage._queueReceive.Receive();

                Communication.Message messageTwo = (Communication.Message)messageRead.Body;

                foreach (var item in ProcessingMessage._messageHandler)
                {
                    if (item.Key == messageTwo.TypeMessage)
                    {
                        item.Value.DynamicInvoke(message);
                    }
                }
            }                   
        }
    }
}
