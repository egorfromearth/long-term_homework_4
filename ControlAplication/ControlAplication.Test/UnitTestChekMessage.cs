using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Messaging;
using Communication;

namespace ControlAplication.Test
{
    [TestClass]
    public class UnitTestChekMessage
    {
        [TestMethod]
        public void ChekMessageTestReceiveMessage()
        {
            MessageQueue.Create(@".\private$\ProcessingMessagTest");

            MessageQueue testQueue = new MessageQueue(@".\private$\ProcessingMessagTest");
            testQueue.Formatter = new BinaryMessageFormatter();

            Communication.Message testMessage = new Communication.Message(0, 0);
            testQueue.Send(testMessage);

            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), "Message");

            MessageQueue.Delete(@".\private$\ProcessingMessagTest");
        }

        [TestMethod]
        public void ChekMessageTestReceiveHalloMessage()
        {
            MessageQueue.Create(@".\private$\ProcessingMessagTest");

            MessageQueue testQueue = new MessageQueue(@".\private$\ProcessingMessagTest");
            testQueue.Formatter = new BinaryMessageFormatter();

            SystemInformation info = new SystemInformation();

            HalloMessage testMessage = new HalloMessage(0, 0, info);
            testQueue.Send(testMessage);

            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), "HalloMessage");

            MessageQueue.Delete(@".\private$\ProcessingMessagTest");
            MessageQueue.Delete(@".\private$\secondrecive1");
            MessageQueue.Delete(@".\private$\secondsend1");
        }

        [TestMethod]
        public void ChekMessageTestReceiveTaskMessage()
        {
            MessageQueue.Create(@".\private$\ProcessingMessagTest");

            MessageQueue testQueue = new MessageQueue(@".\private$\ProcessingMessagTest");
            testQueue.Formatter = new BinaryMessageFormatter();
            Task task = new Task("",0);

            TaskMessage testMessage = new TaskMessage(0, 0, task, "a", "b");
            testQueue.Send(testMessage);

            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), "TaskMessage");

            MessageQueue.Delete(@".\private$\ProcessingMessagTest");
        }

        [TestMethod]
        public void ChekMessageTestExitMessage()
        {
            MessageQueue.Create(@".\private$\ProcessingMessagTest");

            MessageQueue testQueue = new MessageQueue(@".\private$\ProcessingMessagTest");
            testQueue.Formatter = new BinaryMessageFormatter();

            ExitMessage testMessage = new ExitMessage(0, 0);
            testQueue.Send(testMessage);

            Assert.AreEqual(ProcessingMessage.ChekMessage(testQueue), "ExitMessage");

            MessageQueue.Delete(@".\private$\ProcessingMessagTest");
        }
    }
}
