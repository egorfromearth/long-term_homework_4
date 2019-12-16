using System;
using System.Messaging;
using Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;

namespace Agent.Tests
{
    [TestClass]
    public class ProcessingMessageTest
    {

        [TestMethod]
        public void ChekMessageMessageNotExist()
        {
            if (!MessageQueue.Exists(@".\private$\ChekMessageMessageNotExist"))
            {
                MessageQueue.Create(@".\private$\ChekMessageMessageNotExist");
            }

            MessageQueue testQueue = new MessageQueue(@".\private$\ChekMessageMessageNotExist");
            testQueue.Formatter = new BinaryMessageFormatter();

            int message = 15;
            testQueue.Send(message);

            Assert.AreEqual(-1, ProcessingMessage.ChekMessage(testQueue));

            MessageQueue.Delete(@".\private$\ChekMessageMessageNotExist");
        }

        [TestMethod]
        public void ChekMessageTaskStopProcessingMessage()
        {
            if (!MessageQueue.Exists(@".\private$\ChekMessageTaskStopProcessingMessage"))
            {
                MessageQueue.Create(@".\private$\ChekMessageTaskStopProcessingMessage");
            }

            MessageQueue testQueue = new MessageQueue(@".\private$\ChekMessageTaskStopProcessingMessage");
            testQueue.Formatter = new BinaryMessageFormatter();

            Task task = new Task("test", 1);
            List<Thread> listThread = new List<Thread>();

            Agent.tasksList.Add(task);
            Agent.threadDictionary.Add(1, listThread);

            TaskStopProcessingMessage testMessage = new TaskStopProcessingMessage(0, 0, 1);
            testQueue.Send(testMessage);

            Assert.AreEqual((int)EnumTypeMessage.TaskStopProcessingMessage, ProcessingMessage.ChekMessage(testQueue));
            Assert.AreEqual(0, Agent.tasksList.Count);

            MessageQueue.Delete(@".\private$\ChekMessageTaskStopProcessingMessage");
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

        [TestMethod]
        public void ChekMessageTestReceiveTaskMessage()
        {
            if (!MessageQueue.Exists(@".\private$\ChekMessageTestReceiveTaskMessage"))
            {
                MessageQueue.Create(@".\private$\ChekMessageTestReceiveTaskMessage");
            }

            MessageQueue testQueue = new MessageQueue(@".\private$\ChekMessageTestReceiveTaskMessage");
            testQueue.Formatter = new BinaryMessageFormatter();
            Task task = new Task("", 0);

            TaskMessage testMessage = new TaskMessage(0, 0, task, "a", "b");
            testQueue.Send(testMessage);

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

            Agent.QueueSend = testQueue;

            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), (int)EnumTypeMessage.PingMessage);

            MessageQueue.Delete(@".\private$\ChekMessagePingMessagMessage");
        }

        [Ignore]
        [TestMethod]
        public void ChekMessageTestExitMessage()
        {
            if (!MessageQueue.Exists(@".\private$\ChekMessageTestExitMessage"))
            {
                MessageQueue.Create(@".\private$\ChekMessageTestExitMessage");
            }

            MessageQueue testQueue = new MessageQueue(@".\private$\ChekMessageTestExitMessage");
            testQueue.Formatter = new BinaryMessageFormatter();

            ExitMessage testMessage = new ExitMessage(0, 0);
            testQueue.Send(testMessage);
            Agent.QueueSend = testQueue;

            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), (int)EnumTypeMessage.ExitMessage);

            MessageQueue.Delete(@".\private$\ChekMessageTestExitMessage");
        }


        private static void testConnectThread() {
            MessageQueue queue = new MessageQueue(Environment.MachineName + @"\private$\testConnecting");
            queue.Formatter = new BinaryMessageFormatter();
            var message = queue.Receive();
            if (((Communication.Message)message.Body).TypeMessage == (int)Communication.EnumTypeMessage.HelloMessage) {
                CreateQuereMessage createQuereMessage = new CreateQuereMessage(0, 0, Environment.MachineName + @"\private$\testConnectingSend", Environment.MachineName + @"\private$\testConnectingRecive");
                queue.Send(createQuereMessage);
            }
        }

        [TestMethod]
        public void ConnectingTest()
        {
            if (!MessageQueue.Exists(Environment.MachineName + @"\private$\testConnecting"))
            {
                MessageQueue.Create(Environment.MachineName + @"\private$\testConnecting");
            }
            if (!MessageQueue.Exists(Environment.MachineName + @"\private$\testConnectingSend"))
            {
                MessageQueue.Create(Environment.MachineName + @"\private$\testConnectingSend");
            }
            if (!MessageQueue.Exists(Environment.MachineName + @"\private$\testConnectingRecive"))
            {
                MessageQueue.Create(Environment.MachineName + @"\private$\testConnectingRecive");
            }
            Thread testThread = new Thread(testConnectThread);
            testThread.Start();

            Assert.IsTrue(ProcessingMessage.Connecting(Environment.MachineName+@"\private$\testConnecting"));

            MessageQueue.Delete(@".\private$\testConnecting");
            MessageQueue.Delete(@".\private$\testConnectingSend");
            MessageQueue.Delete(@".\private$\testConnectingRecive");
        }
    }
}
