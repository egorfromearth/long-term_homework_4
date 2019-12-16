using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading;
using Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ControlApplication.Tests
{
    [TestClass]
    public class UnitProcessingMessage
    {
        [TestMethod]
        public void ChekMessageTestExitMessage()
        {
            if (!MessageQueue.Exists(@".\private$\ChekMessageTestExitMessage"))
            {
                MessageQueue.Create(@".\private$\ChekMessageTestExitMessage");
            }

            if (!MessageQueue.Exists(@".\private$\ChekMessageTestExitSendMessage"))
            {
                MessageQueue.Create(@".\private$\ChekMessageTestExitSendMessage");
            }

            if (!MessageQueue.Exists(@".\private$\ChekMessageTestExitReciveMessage"))
            {
                MessageQueue.Create(@".\private$\ChekMessageTestExitReciveMessage");
            }

            MessageQueue testQueue = new MessageQueue(@".\private$\ChekMessageTestExitMessage");
            testQueue.Formatter = new BinaryMessageFormatter();

            ExitMessage testMessage = new ExitMessage(0, 0);
            testQueue.Send(testMessage);

            Agent agent = new Agent(0, null, @".\private$\ChekMessageTestExitSendMessage", @".\private$\ChekMessageTestExitReciveMessage");
            ProcessingMessage.agentsList.Add(agent);
            agent.SetConnect();

            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), (int)EnumTypeMessage.ExitMessage);

            MessageQueue.Delete(@".\private$\ChekMessageTestExitMessage");
        }
        [TestMethod]
        public void ChekMessageTestEmptyMessage()
        {
            if (!MessageQueue.Exists(@".\private$\ChekMessageTestEmptyMessage"))
            {
                MessageQueue.Create(@".\private$\ChekMessageTestEmptyMessage");
            }

            MessageQueue testQueue = new MessageQueue(@".\private$\ChekMessageTestEmptyMessage");
            testQueue.Formatter = new BinaryMessageFormatter();

            Communication.Message testMessage = new Communication.Message(0, 0);
            testQueue.Send(testMessage);

            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), (int)EnumTypeMessage.Message);

            MessageQueue.Delete(@".\private$\ChekMessageTestEmptyMessage");
        }


        private static void testChekMessageReceiveHalloMessage()
        {
            MessageQueue queue = new MessageQueue(@".\private$\ChekMessageTestReceiveHalloMessage");
            queue.Formatter = new BinaryMessageFormatter();
            var message = queue.Receive();

            while (((Communication.Message)message.Body).TypeMessage != (int)EnumTypeMessage.CreateQuereMessage)
            {
                queue.Send(message);
                Thread.Sleep(1000);
                message = queue.Receive();
            }

            MessageQueue queueSend = new MessageQueue(((CreateQuereMessage)message.Body).PathSend);
            queueSend.Formatter = new BinaryMessageFormatter();
            Communication.Message messageSend = new Communication.Message(0,0);
            queueSend.Send(messageSend);

        }

        [TestMethod]
        public void ChekMessageTestHalloMessage()
        {
            if (!MessageQueue.Exists(@".\private$\ChekMessageTestReceiveHalloMessage"))
            {
                MessageQueue.Create(@".\private$\ChekMessageTestReceiveHalloMessage");
            }

            MessageQueue testQueue = new MessageQueue(@".\private$\ChekMessageTestReceiveHalloMessage");
            testQueue.Formatter = new BinaryMessageFormatter();

            SystemInformation info = new SystemInformation();

            HalloMessage testMessage = new HalloMessage(0, 0, info);
            testQueue.Send(testMessage);

            Thread testThread = new Thread(testChekMessageReceiveHalloMessage);
            testThread.Start();

            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), (int)EnumTypeMessage.HelloMessage);

            MessageQueue.Delete(@".\private$\ChekMessageTestReceiveHalloMessage");
            MessageQueue.Delete(@".\private$\secondrecive1");
            MessageQueue.Delete(@".\private$\secondsend1");
        }

        [TestMethod]
        public void ChekMessageTestTaskMessage()
        {
            if (!MessageQueue.Exists(@".\private$\ChekMessageTestReceiveTaskMessage"))
            {
                MessageQueue.Create(@".\private$\ChekMessageTestReceiveTaskMessage");
            }

            MessageQueue testQueue = new MessageQueue(@".\private$\ChekMessageTestReceiveTaskMessage");
            testQueue.Formatter = new BinaryMessageFormatter();
            Task task = new Task("", 0);
            task.Complete = true;

            TaskMessage testMessage = new TaskMessage(0, 0, task, "a", "b");
            testQueue.Send(testMessage);

            ProcessingMessage.tasksList.Add(task);
            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), (int)EnumTypeMessage.TaskMessage);

            MessageQueue.Delete(@".\private$\ChekMessageTestReceiveTaskMessage");
        }

        [TestMethod]
        public void ChekMessagePingMessagMessage()
        {
            if (!MessageQueue.Exists(@".\private$\ChekMessagePingMessagMessage"))
            {
                MessageQueue.Create(@".\private$\ChekMessagePingMessagMessage");
            }

            MessageQueue testQueue = new MessageQueue(@".\private$\ChekMessagePingMessagMessage");
            testQueue.Formatter = new BinaryMessageFormatter();

            PingMessage pingMessage = new PingMessage(0, 0);
            testQueue.Send(pingMessage);

            ProcessingMessage.mainQueue = testQueue;

            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), (int)EnumTypeMessage.PingMessage);

            MessageQueue.Delete(@".\private$\ChekMessagePingMessagMessage");
        }

        [TestMethod]
        public void SendMessageArrayTest() {

            SystemInformation info = new SystemInformation();
            info.CountCore = 4;
            info.PasswordsPerSecond = 100000;


            Agent agentOne = new Agent(0, info, @".\private$\SendMessageArrayTestSend1", @".\private$\SendMessageArrayTestReceive1");
            Agent agentTwo = new Agent(0, info, @".\private$\SendMessageArrayTestSend2", @".\private$\SendMessageArrayTestReceive2");
            Agent agentThree = new Agent(0, info, @".\private$\SendMessageArrayTestSend3", @".\private$\SendMessageArrayTestReceive3");

            if (!MessageQueue.Exists(agentOne.QueueReceiveName))
            {
                MessageQueue.Create(agentOne.QueueReceiveName);
            }

            if (!MessageQueue.Exists(agentTwo.QueueReceiveName))
            {
                MessageQueue.Create(agentTwo.QueueReceiveName);
            }

            if (!MessageQueue.Exists(agentThree.QueueReceiveName))
            {
                MessageQueue.Create(agentThree.QueueReceiveName);
            }

            agentOne.SetConnect();
            agentTwo.SetConnect();
            agentThree.SetConnect();

            Communication.Message testMessage = new Communication.Message(0, 0);
            PingMessage pingMessage = new Communication.PingMessage(0, 0);

            Communication.Message[] arrayMessage = { testMessage, pingMessage };

            List<Agent> agentList = new List<Agent>() { agentOne, agentTwo, agentThree };

            ProcessingMessage.SendMessageArray(arrayMessage, agentList);

            foreach (var itemAgent in agentList) {
                var messageObj = itemAgent.QueueReceive.Receive();
                Communication.Message message = (Communication.Message)messageObj.Body;
                Assert.AreEqual(message.TypeMessage, (int)EnumTypeMessage.Message);

                messageObj = itemAgent.QueueReceive.Receive();
                message = (Communication.Message)messageObj.Body;
                Assert.AreEqual(message.TypeMessage, (int)EnumTypeMessage.PingMessage);
            }

            MessageQueue.Delete(@".\private$\SendMessageArrayTestReceive1");
            MessageQueue.Delete(@".\private$\SendMessageArrayTestReceive2");
            MessageQueue.Delete(@".\private$\SendMessageArrayTestReceive3");
        }

    }
}
