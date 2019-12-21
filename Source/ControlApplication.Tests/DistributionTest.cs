using System;
using System.Collections.Generic;
using Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Messaging;

namespace ControlApplication.Tests
{
    [TestClass]
    public class UnitDistribution
    {
        [TestMethod]
        public void GetFreeAgentTest()
        {
            SystemInformation info = new SystemInformation();
            info.CountCore = 4;
            info.PasswordsPerSecond = 100000;

            Agent agentOne = new Agent(1, info, "test", "test");
            Agent agentTwo = new Agent(2, info, "test", "test");
            agentTwo.Connect = true;

            List<Agent> listAgent = new List<Agent>();
            listAgent.Add(agentOne);
            listAgent.Add(agentTwo);

            var IdAgent = DistributionRanges.GetFreeAgent(listAgent);

            Assert.AreEqual(IdAgent, 2);
        }

        [TestMethod]
        public void lineShift()
        {
            ulong len = 10;
            string start = "AAA";
            string stop = "AAK";
            Assert.AreEqual(stop, DistributionRanges.lineShift(start, len));
        }
        [TestMethod]
        public void DistributionTest()
        {
            List<Agent> listAgent = new List<Agent>();
            Task task = new Task("YpAs3o52zp/pP1F65AlheA==", 0);
            SystemInformation info = new SystemInformation();
            info.CountCore = 4;
            info.PasswordsPerSecond = 100000;
            Agent agent = new Agent(0, info, "test", "test");
            agent.Connect = true;
            listAgent.Add(agent);

            var messageArr = DistributionRanges.Distribution(ref listAgent, task);

            string[] correctRange = { "A", "000000" };
            string[] range = { messageArr[0].Start, messageArr[0].Stop };

            Assert.AreEqual(range[0], correctRange[0]);
            Assert.AreEqual(range[1], correctRange[1]);
        }

      
        [TestMethod]
        public void DisconnectAgentHandlerExistAgent()
        {

            if (!MessageQueue.Exists(@".\private$\DisconnectAgentHandlerTestSend"))
            {
                MessageQueue.Create(@".\private$\DisconnectAgentHandlerTestSend");
            }

            if (!MessageQueue.Exists(@".\private$\DisconnectAgentHandlerTestReceive"))
            {
                MessageQueue.Create(@".\private$\DisconnectAgentHandlerTestReceive");
            }

            Dictionary<Task, List<string[]>> tasks = new Dictionary<Task, List<string[]>>();
            Task task = new Task("test", 1);
            string[] testSting = { "a", "000000" };

            List<string[]> testList = new List<string[]>();
            testList.Add(testSting);
            tasks.Add(task, testList);

            SystemInformation info = new SystemInformation();
            info.CountCore = 4;
            info.PasswordsPerSecond = 4;
            Agent agent = new Agent(0, info, @".\private$\DisconnectAgentHandlerTestSend", @".\private$\DisconnectAgentHandlerTestReceive");
            agent.SetConnect();
            List<Agent> list = new List<Agent>();
            list.Add(agent);
            DistributionRanges.DisconnectAgentHandler(tasks, ref list);

            Assert.AreEqual(3, list[0].Info.CountCore);
            Assert.AreEqual((int)Communication.EnumTypeMessage.TaskMessage, ((Communication.Message)agent.QueueReceive.Receive().Body).TypeMessage);


            MessageQueue.Delete(@".\private$\DisconnectAgentHandlerTestSend");
            MessageQueue.Delete(@".\private$\DisconnectAgentHandlerTestReceive");
        }
                          

        [TestMethod]
        public void DisconnectAgentHandlerNotExistAgent()
        {

            if (!MessageQueue.Exists(@".\private$\DisconnectAgentHandlerTestSend"))
            {
                MessageQueue.Create(@".\private$\DisconnectAgentHandlerTestSend");
            }

            if (!MessageQueue.Exists(@".\private$\DisconnectAgentHandlerTestReceive"))
            {
                MessageQueue.Create(@".\private$\DisconnectAgentHandlerTestReceive");
            }

            Dictionary<Task, List<string[]>> tasks = new Dictionary<Task, List<string[]>>();
            Task task = new Task("test", 1);
            string[] testSting = { "a", "000000" };

            List<string[]> testList = new List<string[]>();
            testList.Add(testSting);
            tasks.Add(task, testList);

            SystemInformation info = new SystemInformation();
            info.CountCore = 0;
            info.PasswordsPerSecond = 4;
            Agent agent = new Agent(0, info, @".\private$\DisconnectAgentHandlerTestSend", @".\private$\DisconnectAgentHandlerTestReceive");
            agent.SetConnect();
            agent.Tasks.Add(task, testList);

            List<Agent> list = new List<Agent>();
            list.Add(agent);
            DistributionRanges.DisconnectAgentHandler(tasks, ref list);

            Assert.AreEqual(1, list[0].Info.CountCore);
            Assert.AreEqual((int)Communication.EnumTypeMessage.TaskStopProcessingMessage, ((Communication.Message)agent.QueueReceive.Receive().Body).TypeMessage);


            MessageQueue.Delete(@".\private$\DisconnectAgentHandlerTestSend");
            MessageQueue.Delete(@".\private$\DisconnectAgentHandlerTestReceive");
        }
    }
}
