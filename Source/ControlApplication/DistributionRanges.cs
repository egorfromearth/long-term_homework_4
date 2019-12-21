using Communication;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace ControlApplication
{
    public class DistributionRanges
    {
        private static char[] charactersToTest =
        {
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'А','Б','В','Г','Д','Е','Ё','Ж','З','И','К','Л','М','Н','О','П','Р','С','Т','У','Ф','Х','Ц','Ч','Ш','Щ','Ъ','Ы','Ь','Э','Ю','Я',
            'а','б','в','г','д','е','ё','ж','з','и','к','л','м','н','о','п','р','с','т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я',
            '1','2','3','4','5','6','7','8','9','0'
        };

        public static int GetFreeAgent(List<Agent> agentsList)
        {

            foreach (var item in agentsList)
            {
                if (item.Info.CountCore > 0 && item.Connect)
                {
                    return item.IdAgent;
                }
            }
            return -1;
        }

        public static void DisconnectAgentHandler(Dictionary<Task, List<string[]>> tasks, ref List<Agent> agentsList)
        {
            foreach (var itemTask in tasks)
            {
                foreach (var itemSegment in itemTask.Value)
                {
                    var idAgent = GetFreeAgent(agentsList);

                    if (idAgent != -1)
                    {
                        TaskMessage taskMessage = new TaskMessage(idAgent, 0, itemTask.Key, itemSegment[0], itemSegment[1]);

                        for (int index = 0; index < agentsList.Count; ++index)
                        {
                            if (agentsList[index].IdAgent == idAgent)
                            {
                                string[] tempSegment = { itemSegment[0], itemSegment[1] };

                                if (agentsList[index].Tasks.ContainsKey(itemTask.Key))
                                {
                                    agentsList[index].Tasks[itemTask.Key].Add(tempSegment);
                                }
                                else
                                {
                                    List<string[]> newList = new List<string[]> { tempSegment };
                                    agentsList[index].Tasks.Add(itemTask.Key, newList);
                                }

                                agentsList[index].Info.CountCore--;
                                agentsList[index].QueueReceive.Send(taskMessage);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ошибка распределения HASH:" + itemTask.Key.Hash + " (недостаточно агентов) введите задание заново.");

                        for (int index = 0; index < agentsList.Count; ++index)
                        {
                            if (agentsList[index].Tasks.ContainsKey(itemTask.Key))
                            {
                                TaskStopProcessingMessage taskStopProcessing = new TaskStopProcessingMessage(agentsList[index].IdAgent, 0, itemTask.Key.IdTask);
                                agentsList[index].QueueReceive.Send(taskStopProcessing);
                                agentsList[index].Info.CountCore++;
                            }
                        }
                        break;
                    }
                }
            }
        }
        public static string reverseStr(string str)
        {
            if (str.Length == 1)
                return str;
            char[] inputarray = str.ToCharArray();
            Array.Reverse(inputarray);
            string output = new string(inputarray);
            return output;
        }
        /// <summary>
        /// Сдвигает последовательность букв на заданную длину.
        /// </summary>
        /// <param name="start">Стартовая строка, которую мы будем двигать.</param>
        /// <param name="len">Число раз, на которое будем двигать.</param>
        public static string lineShift(string start, ulong len)
        {
            ulong decStart = 0;

            int[] indexes = new int[start.Length];
            for (int index = 0; index < start.Length; ++index)
                indexes[index] = Array.IndexOf(charactersToTest, start[index]);

            for (int index = 0; index < start.Length; ++index)
                decStart += (ulong)(Math.Pow(charactersToTest.Length, index) * (indexes[index] + 1));

            decStart += len;
            ulong temp = decStart;
            int count = 0;
            while (temp != 0)
            {
                temp /= (ulong)charactersToTest.Length;
                ++count;
            }
            string res = "";
            StringBuilder result = new StringBuilder(res, 6);
            for (int index = 0; index < count; ++index)
            {
                int num = (int)(decStart / Math.Pow(charactersToTest.Length, count - index - 1));
                result.Append(charactersToTest[num - 1]);
                decStart %= (ulong)Math.Pow(charactersToTest.Length, count - index - 1);
            }

            return result.ToString();
        }

        public static TaskMessage[] Distribution(ref List<Agent> agentsList, Task task)
        {
            List<Agent> agentsListConnect = new List<Agent>();

            for (int index = 0; index < agentsList.Count; ++index)
            {
                if (agentsList[index].Connect && agentsList[index].Info.CountCore > 0)
                {
                    agentsListConnect.Add(agentsList[index]);
                    agentsList[index].Info.CountCore--;
                }
            }

            ulong[] speedArr = new ulong[agentsListConnect.Count];

            for (int i = 0; i < agentsListConnect.Count; ++i)
            {
                speedArr[i] = (ulong)agentsListConnect[i].Info.PasswordsPerSecond;
            }

            if (speedArr.Length > 0)
            {

                string[,] myArr = new string[speedArr.Length, 2];
                ulong[] count_pass = new ulong[speedArr.Length];
                ulong sum = 0;

                for (int index = 0; index < speedArr.Length; ++index)
                    sum += speedArr[index];

                const ulong countCombination = 3664769671500;
                ulong time = countCombination / sum;
                string start = "A";
                for (int index = 0; index < speedArr.Length - 1; ++index)
                {

                    myArr[index, 0] = start;
                    myArr[index, 1] = lineShift(reverseStr(myArr[index, 0]), (time * speedArr[index]));
                    start = lineShift(reverseStr(myArr[index, 1]), 1);
                }

                if (speedArr.Length != 1)
                {
                    myArr[speedArr.Length - 1, 0] = lineShift(reverseStr(myArr[speedArr.Length - 2, 1]), 1);
                    myArr[speedArr.Length - 1, 1] = "000000";
                }
                else
                {
                    myArr[0, 0] = "A";
                    myArr[0, 1] = "000000";
                }

                TaskMessage[] messsageArr = new TaskMessage[agentsListConnect.Count];

                for (int index = 0; index < agentsListConnect.Count; ++index)
                {
                    string[] range = new string[2] { myArr[index, 0], myArr[index, 1] };

                    if (agentsListConnect[index].Tasks.ContainsKey(task))
                    {
                        agentsListConnect[index].Tasks[task].Add(range);
                    }
                    else
                    {
                        List<string[]> newList = new List<string[]> { range };
                        agentsListConnect[index].Tasks.Add(task, newList);
                    }

                    TaskMessage message = new TaskMessage(agentsListConnect[index].IdAgent, 0, task, myArr[index, 0], myArr[index, 1]);
                    messsageArr[index] = message;
                }
                return messsageArr;
            }
            Console.WriteLine("Ошибка распределения HASH: "+task.Hash+ " (недостаточно агентов) введите задание заново." );
            return null;
        }
    }
}
