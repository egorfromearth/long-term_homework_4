using Communication;
using ControlApplication;
using System;
using System.Collections.Generic;
using System.Messaging;

namespace ControlApplication
{
    public class Agent
    {
        public int IdAgent { get; private set; }
        public bool Connect { get; set; }
        public string QueueSendName { get; private set; }
        public string QueueReceiveName { get; private set; }
        public DateTime LastConnect { get; set; }
        public MessageQueue QueueSend { get; private set; }
        public MessageQueue QueueReceive { get; private set; }
        public Dictionary<Task, List<string[]>> Tasks { get; set; }
        public SystemInformation Info { get; private set; }

        public delegate void DisconnectAgentHandler(Dictionary<Task, List<string[]>> tasks, ref List <Agent> agentsList);
        public event DisconnectAgentHandler disconnectAgent;

        public void PingAgent()
        {
            PingMessage pingMessage = new PingMessage(IdAgent, 0);
            while (true)
            {
                if ((DateTime.Now - LastConnect).TotalSeconds > 8)
                {
                    if (Connect)
                    {
                        Connect = false;
                        disconnectAgent(Tasks, ref ProcessingMessage.agentsList);
                        Console.WriteLine("Соединение с агеном IdAgent: " + IdAgent + " потеряно");
                    }                         
                }
                else {
                    if (!Connect) {
                        Console.WriteLine("Соединение с агеном IdAgent: " + IdAgent + " восстановлено");
                    }
                    Connect = true;
                }
                System.Threading.Thread.Sleep(5000);
                QueueReceive.Send(pingMessage);
            }
        }

        public void WorkingAgent()
        {
            while (true)
            {
                ProcessingMessage.ChekMessage(QueueSend);
            }
        }

        public bool SetConnect() {
            try
            {
                Connect = true;
                QueueSend = new MessageQueue(QueueSendName);
                QueueSend.Formatter = new BinaryMessageFormatter();
                QueueReceive = new MessageQueue(QueueReceiveName);
                QueueReceive.Formatter = new BinaryMessageFormatter();
                disconnectAgent += DistributionRanges.DisconnectAgentHandler;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public Agent(int idAgent, SystemInformation info, string queueSend, string queueReceive)
        {
            IdAgent = idAgent;
            QueueSendName = queueSend;
            QueueReceiveName = queueReceive;
            Info = info;
            LastConnect = DateTime.Now;
            Tasks = new Dictionary<Task, List<string[]>>();
            Connect = false;
        }
    }
}