using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography; //Для MD5
using System.Threading;

namespace Agent
{


    public class PasswordGuessing
    {
        private static char[] charactersToTest =
                {
            'A','B','C', 'D', 'E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v', 'w','x','y','z',
            'А','Б','В','Г','Д','Е','Ё','Ж','З','И','К','Л','М','Н','О','П','Р','С','Т','У','Ф','Х','Ц','Ч','Ш','Щ','Ъ','Ы','Ь','Э','Ю','Я',
            'а','б','в','г','д','е','ё','ж','з','и','к','л','м','н','о','п','р','с','т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я',
            '1','2','3','4','5','6','7','8','9','0'
        };

        public static string pass = "";

        /// <summary>
        /// Возвращает хеш строки
        /// </summary>
        /// <param name="input">строка, хэш которой нужно получить</param>
        /// <returns>Строку, являющуюся хешем строки</returns>
        public static string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Функция для проведения теста на скорость подбора паролей
        /// </summary>
        /// <returns>Приведенное к целому числу количество паролей, подбираемых ПК в секунду</returns>
        public int SpeedTest()
        {
            var timeStarted = DateTime.Now;
            Brute("AAA", "BBB", "BBB");
            double speed_buf = ((124 * 124 + 124 + 1) / (DateTime.Now.Subtract(timeStarted).TotalSeconds));
            int speed = (int)speed_buf;
            return speed;
        }

        /// <summary>
        /// Делает подбор пароля от start До stop размера n
        /// </summary>
        /// <param name="n">Длина подбираемого пароля</param>
        /// <returns>True - если пароль найдет, false - если пароль не нашли. </returns>
        public static bool BruteForce(int n, string start, string stop, string hash)
        {
            int imin = 0;
            int imax = charactersToTest.Length - 1;
            int[] indexes = new int[n];

            for (int index = 0; index < n; ++index)
                indexes[index] = Array.IndexOf(charactersToTest, start[index]);


            while (indexes[n - 1] != imax + 1)
            {
                string pass_buf = "";
                StringBuilder pass_buff = new StringBuilder(pass_buf, 6);


                for (int index = 0; index < n; ++index)
                    pass_buff.Append(charactersToTest[indexes[index]]); 

                pass_buf = pass_buff.ToString();

                if (GetHash(pass_buf) == hash)
                {
                    pass = pass_buf;
                    return true;
                }

                if (pass_buf == stop)
                    return false;

                ++indexes[0];

                for (int index = 0; index < n - 1; ++index)
                {
                    if (indexes[index] > imax)
                    {
                        indexes[index] = imin;
                        ++indexes[index + 1];
                    }
                }
            }

            return false;

        }



        /// <summary>
        /// Сдвигает последовательность букв на заданную длину.
        /// </summary>
        /// <param name="start">Стартовая строка, которую мы будем двигать.</param>
        /// <param name="len">Число, на которое будем двигать.</param>
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

            string r = result.ToString();
            return r;


        }


        /// <summary>
        /// Функция подобра пароля.
        /// </summary>
        /// <param name="start">Диапазон, с которого начинается поиск</param>
        /// <param name="stop">Конечная граница поиска</param>
        /// <param name="pswd">Свертка</param>
        /// <returns>Найде/не найден пароль</returns>
        public void Brute(string start, string stop, string pswd)
        {
            bool flag = false;
            string[] startArr = { "A", "AA", "AAA", "AAAA", "AAAAA", "AAAAAA" };
            string[] stopArr = { "0", "00", "000", "0000", "00000", "000000" };
            int startLen = start.Length;
            int stopLen = stop.Length;

            if (startLen != stopLen)
            {
                flag = BruteForce(startLen, start, stopArr[startLen - 1], pswd);
                ++startLen;
            }


            while ((startLen != stopLen + 1) && !flag)
            {

                if (startLen == stopLen)
                    flag = BruteForce(startLen, startArr[startLen - 1], stop, pswd);
                else
                    flag = BruteForce(startLen, startArr[startLen - 1], stopArr[startLen - 1], pswd);

                ++startLen;
            }

        }

    }
}

