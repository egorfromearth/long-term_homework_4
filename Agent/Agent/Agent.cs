using System;
using System.Threading;

namespace Agent
{

    class Agent
    {
              
        private static void SetInfo() {
            PasswordGuessing temp = new PasswordGuessing();
            ProcessingMessage._info.CountCore = Environment.ProcessorCount;
            ProcessingMessage._info.PasswordsPerSecond = temp.SpeedTest(); //надо как то узнать скорость подбора
        }

        static void Main(string[] args)
        {
            SetInfo();
            if (!ProcessingMessage.Connecting()) {
                Environment.Exit(1);
            }

            while (true) {

                var messageRead = ProcessingMessage._queueReceive.Receive();

                Communication.Message message = (Communication.Message)messageRead.Body;

                foreach (var item in ProcessingMessage._messageHandler)
                {
                    if (item.Key == message.TypeMessage)
                    {
                        item.Value.DynamicInvoke(message);
                    }
                }
            }                   
        }
    }
}
