using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography; //Для MD5

namespace Agent
{


    class PasswordGuessing
    {

        private static char[] alphabet = { 'A','B','C','D','E','F','G','H','I','J','C','L','M','N','O','P','Q','R','S','T','U','V','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','x','y','z',
            'А','Б','В','Г','Д','Е','Ё','Ж','З','И','К','Л','М','Н','О','П','Р','С','Т','У','Ф','Х','Ц','Ч','Ш','Щ','Ъ','Ы','Ь','Э','Ю','Я',
            'а','б','в','г','д','е','ё','ж','з','и','к','л','м','н','о','п','р','с','т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я',
            '1','2','3','4','5','6','7','8','9','0'};


        private static string pswd = "CBDF";

        private static string pass = "";
        private static char[] start = { '0', '0', '7' };
        private static char[] stop = { 'a', 'B', 'C', 'D' };
        private static int length = alphabet.Length;
        public static string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        private static string hash = GetHash(pswd);

        /// <summary>
        /// Переводит строку типа string в массив символов char[].
        /// </summary>
        /// <param name="str"> Строка, которую нужно перевести. </param>
        /// <returns> Массив символов строки str.</returns>
        public static char[] StringToCharArr(string str)
        {
            char[] mas = new char[str.Length];
            for (int i = 0; i < str.Length; ++i)
                mas[i] = str[i];
            return mas;
        }

        /// <summary>
        /// Проверяет два символьных массива на равенство.
        /// </summary>
        /// <param name="mas1">Первый массив</param>
        /// <param name="mas2">Второй массив</param>
        /// <returns> true - массивы равны, false - массивы не равны</returns>
        public static bool Equal(char[] mas1, char[] mas2)
        {
            int len1 = mas1.Length;
            int len2 = mas2.Length;

            if (len1 != len2)
                return false;

            for (int index = 0; index < len1; ++index)
            {
                if (mas1[index] != mas2[index])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Сравнивает текущий пароль с искомым и сравнивает текущий пароль с граничным, для выхода из функции поиска.
        /// </summary>
        /// <param name="mas"> Текущий пароль. </param>
        public static bool hashComparison(char[] mas, char[] stop)
        {
            char[] hash_buff = StringToCharArr(hash);
            if (GetHash(new string(mas)) == hash)
            {
                Console.WriteLine("Pass find!");
                Console.WriteLine(mas);
                pass = new string(mas);

                System.Environment.Exit(0);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ищет индексы букв из стартовой последовательости в массиве-алфавите.
        /// </summary>
        /// <param name="start"> Стартовая последовательность </param>
        /// <returns> Массив индексов. </returns>
        private static int[] FindIndex(char[] start)
        {
            int[] mas = new int[start.Length];

            for (int i = 0; i < start.Length; ++i)
                mas[i] = Array.IndexOf(alphabet, start[i], 0);

            return mas;
        }


        /// <summary>
        /// Подбирает пароль из 2-х символов
        /// </summary>
        /// <param name="start"> Стартовая последовательность </param>
        /// <param name="stop"> Конечная последовательность </param>
        /// <returns> False - пароль не найден. true - пароль найден. </returns>
        private static bool BruteForceOnTwoSymbol(char[] start, char[] stop)
        {
            int[] arr = new int[start.Length];
            char[] arr_string = new char[2];
            arr = FindIndex(start);
            for (int i = arr[0]; i < length; ++i)
            {
                if (i == arr[0])
                {
                    for (int j = arr[1]; j < length; ++j)
                    {
                        arr_string[0] = alphabet[i];
                        arr_string[1] = alphabet[j];
                        hashComparison(arr_string, stop);
                    }
                }
                else
                {
                    for (int j = 0; j < length; ++j)
                    {
                        arr_string[0] = alphabet[i];
                        arr_string[1] = alphabet[j];
                        hashComparison(arr_string, stop);
                    }
                }

            }


            return false;
        }

        /// <summary>
        /// Подбирает пароль из 3-х символов
        /// </summary>
        /// <param name="start"> Стартовая последовательность </param>
        /// <param name="stop"> Конечная последовательность </param>
        /// <returns> False - пароль не найден. true - пароль найден. </returns> 
        private static bool BruteForceOnThreeSymbol(char[] start, char[] stop)
        {
            int[] arr = new int[start.Length];
            char[] arr_string = new char[3];
            arr = FindIndex(start);

            for (int i = arr[0]; i < length; ++i)
            {
                if (i == arr[0])
                {
                    for (int j = arr[1]; j < length; ++j)
                    {
                        if (j == arr[1])
                        {
                            for (int k = arr[2]; k < length; ++k)
                            {
                                arr_string[0] = alphabet[i];
                                arr_string[1] = alphabet[j];
                                arr_string[2] = alphabet[k];
                                hashComparison(arr_string, stop);
                            }
                        }
                        else
                        {
                            for (int k = 0; k < length; ++k)
                            {
                                arr_string[0] = alphabet[i];
                                arr_string[1] = alphabet[j];
                                arr_string[2] = alphabet[k];
                                hashComparison(arr_string, stop);
                            }

                        }
                    }
                }
                else
                {
                    for (int j = 0; j < length; ++j)
                    {

                        for (int k = 0; k < length; ++k)
                        {
                            arr_string[0] = alphabet[i];
                            arr_string[1] = alphabet[j];
                            arr_string[2] = alphabet[k];
                            hashComparison(arr_string, stop);
                        }
                    }

                }
            }

            return false;

        }

        /// <summary>
        /// Подбирает пароль из 4-х символов
        /// </summary>
        /// <param name="start"> Стартовая последовательность </param>
        /// <param name="stop"> Конечная последовательность </param>
        /// <returns> False - пароль не найден. true - пароль найден. </returns> 
        private static bool BruteForceOnFourSymbol(char[] start, char[] stop)
        {
            int[] arr = new int[start.Length];
            char[] arr_string = new char[4];
            arr = FindIndex(start);

            for (int i = arr[0]; i < length; ++i)
            {
                if (i == arr[0])
                {
                    for (int j = arr[1]; j < length; ++j)
                    {
                        if (j == arr[1])
                        {
                            for (int k = arr[2]; k < length; ++k)
                            {
                                if (k == arr[2])
                                {

                                    for (int l = arr[3]; l < length; ++l)
                                    {

                                        arr_string[0] = alphabet[i];
                                        arr_string[1] = alphabet[j];
                                        arr_string[2] = alphabet[k];
                                        arr_string[3] = alphabet[l];
                                        hashComparison(arr_string, stop);
                                    }
                                }
                                else
                                {
                                    for (int l = 0; l < length; ++l)
                                    {

                                        arr_string[0] = alphabet[i];
                                        arr_string[1] = alphabet[j];
                                        arr_string[2] = alphabet[k];
                                        arr_string[3] = alphabet[l];
                                        hashComparison(arr_string, stop);
                                    }
                                }



                            }
                        }
                        else
                        {
                            for (int k = 0; k < length; ++k)
                            {
                                for (int l = 0; l < length; ++l)
                                {

                                    arr_string[0] = alphabet[i];
                                    arr_string[1] = alphabet[j];
                                    arr_string[2] = alphabet[k];
                                    arr_string[3] = alphabet[l];
                                    hashComparison(arr_string, stop);
                                }
                            }

                        }
                    }
                }
                else
                {
                    for (int j = 0; j < length; ++j)
                    {

                        for (int k = 0; k < length; ++k)
                        {
                            for (int l = 0; l < length; ++l)
                            {

                                arr_string[0] = alphabet[i];
                                arr_string[1] = alphabet[j];
                                arr_string[2] = alphabet[k];
                                arr_string[3] = alphabet[l];
                                hashComparison(arr_string, stop);
                            }
                        }
                    }

                }
            }

            return false;

        }

        /// <summary>
        /// Подбирает пароль из 5-х символов
        /// </summary>
        /// <param name="start"> Стартовая последовательность </param>
        /// <param name="stop"> Конечная последовательность </param>
        /// <returns> False - пароль не найден. true - пароль найден. </returns>
        private static bool BruteForceOnFiveSymbol(char[] start, char[] stop)
        {
            int[] arr = new int[start.Length];
            char[] arr_string = new char[5];
            arr = FindIndex(start);

            for (int i = arr[0]; i < length; ++i)
            {
                if (i == arr[0])
                {
                    for (int j = arr[1]; j < length; ++j)
                    {
                        if (j == arr[1])
                        {
                            for (int k = arr[2]; k < length; ++k)
                            {
                                if (k == arr[2])
                                {
                                    for (int l = arr[3]; l < length; ++l)
                                    {
                                        if (l == arr[3])
                                        {
                                            for (int m = arr[4]; m < length; ++m)
                                            {
                                                arr_string[0] = alphabet[i];
                                                arr_string[1] = alphabet[j];
                                                arr_string[2] = alphabet[k];
                                                arr_string[3] = alphabet[l];
                                                arr_string[4] = alphabet[m];
                                                hashComparison(arr_string, stop);
                                            }
                                        }
                                        else
                                        {

                                            for (int m = 0; m < length; ++m)
                                            {
                                                arr_string[0] = alphabet[i];
                                                arr_string[1] = alphabet[j];
                                                arr_string[2] = alphabet[k];
                                                arr_string[3] = alphabet[l];
                                                arr_string[4] = alphabet[m];
                                                hashComparison(arr_string, stop);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int l = 0; l < length; ++l)
                                    {


                                        for (int m = 0; m < length; ++m)
                                        {
                                            arr_string[0] = alphabet[i];
                                            arr_string[1] = alphabet[j];
                                            arr_string[2] = alphabet[k];
                                            arr_string[3] = alphabet[l];
                                            arr_string[4] = alphabet[m];
                                            hashComparison(arr_string, stop);
                                        }
                                    }
                                }



                            }
                        }
                        else
                        {
                            for (int k = 0; k < length; ++k)
                            {
                                for (int l = 0; l < length; ++l)
                                {


                                    for (int m = 0; m < length; ++m)
                                    {
                                        arr_string[0] = alphabet[i];
                                        arr_string[1] = alphabet[j];
                                        arr_string[2] = alphabet[k];
                                        arr_string[3] = alphabet[l];
                                        arr_string[4] = alphabet[m];
                                        hashComparison(arr_string, stop);
                                    }
                                }
                            }

                        }
                    }
                }
                else
                {
                    for (int j = 0; j < length; j++)
                    {

                        for (int k = 0; k < length; ++k)
                        {
                            for (int l = 0; l < length; ++l)
                            {


                                for (int m = 0; m < length; ++m)
                                {
                                    arr_string[0] = alphabet[i];
                                    arr_string[1] = alphabet[j];
                                    arr_string[2] = alphabet[k];
                                    arr_string[3] = alphabet[l];
                                    arr_string[4] = alphabet[m];
                                    hashComparison(arr_string, stop);
                                }
                            }
                        }
                    }

                }
            }


            return false;
        }

        /// <summary>
        /// Подбирает пароль из 6-х символов
        /// </summary>
        private static bool BruteForceOnSexSymbol(char[] start, char[] stop)
        {
            int[] arr = new int[start.Length];
            char[] arr_string = new char[6];
            arr = FindIndex(start);

            for (int i = arr[0]; i < length; ++i)
            {
                if (i == arr[0])
                {
                    for (int j = arr[1]; j < length; ++j)
                    {
                        if (j == arr[1])
                        {
                            for (int k = arr[2]; k < length; ++k)
                            {
                                if (k == arr[2])
                                {
                                    for (int l = arr[3]; l < length; ++l)
                                    {
                                        if (l == arr[3])
                                        {
                                            for (int m = arr[4]; m < length; ++m)
                                            {
                                                if (m == arr[4])
                                                {

                                                    for (int n = arr[5]; n < length; ++n)
                                                    {
                                                        arr_string[0] = alphabet[i];
                                                        arr_string[1] = alphabet[j];
                                                        arr_string[2] = alphabet[k];
                                                        arr_string[3] = alphabet[l];
                                                        arr_string[4] = alphabet[m];
                                                        arr_string[5] = alphabet[n];
                                                        hashComparison(arr_string, stop);
                                                    }
                                                }
                                                else
                                                {
                                                    for (int n = 0; n < length; ++n)
                                                    {
                                                        arr_string[0] = alphabet[i];
                                                        arr_string[1] = alphabet[j];
                                                        arr_string[2] = alphabet[k];
                                                        arr_string[3] = alphabet[l];
                                                        arr_string[4] = alphabet[m];
                                                        arr_string[5] = alphabet[n];
                                                        hashComparison(arr_string, stop);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                            for (int m = 0; m < length; ++m)
                                            {
                                                for (int n = 0; n < length; ++n)
                                                {
                                                    arr_string[0] = alphabet[i];
                                                    arr_string[1] = alphabet[j];
                                                    arr_string[2] = alphabet[k];
                                                    arr_string[3] = alphabet[l];
                                                    arr_string[4] = alphabet[m];
                                                    arr_string[5] = alphabet[n];
                                                    hashComparison(arr_string, stop);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int l = 0; l < length; ++l)
                                    {


                                        for (int m = 0; m < length; ++m)
                                        {
                                            for (int n = 0; n < length; ++n)
                                            {
                                                arr_string[0] = alphabet[i];
                                                arr_string[1] = alphabet[j];
                                                arr_string[2] = alphabet[k];
                                                arr_string[3] = alphabet[l];
                                                arr_string[4] = alphabet[m];
                                                arr_string[5] = alphabet[n];
                                                hashComparison(arr_string, stop);
                                            }
                                        }
                                    }
                                }



                            }
                        }
                        else
                        {
                            for (int k = 0; k < length; ++k)
                            {
                                for (int l = 0; l < length; ++l)
                                {


                                    for (int m = 0; m < length; ++m)
                                    {
                                        for (int n = 0; n < length; ++n)
                                        {
                                            arr_string[0] = alphabet[i];
                                            arr_string[1] = alphabet[j];
                                            arr_string[2] = alphabet[k];
                                            arr_string[3] = alphabet[l];
                                            arr_string[4] = alphabet[m];
                                            arr_string[5] = alphabet[n];
                                            hashComparison(arr_string, stop);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                else
                {
                    for (int j = 0; j < length; ++j)
                    {

                        for (int k = 0; k < length; ++k)
                        {
                            for (int l = 0; l < length; ++l)
                            {


                                for (int m = 0; m < length; ++m)
                                {
                                    for (int n = 0; n < length; ++n)
                                    {
                                        arr_string[0] = alphabet[i];
                                        arr_string[1] = alphabet[j];
                                        arr_string[2] = alphabet[k];
                                        arr_string[3] = alphabet[l];
                                        arr_string[4] = alphabet[m];
                                        arr_string[5] = alphabet[n];
                                        hashComparison(arr_string, stop);
                                    }
                                }
                            }
                        }
                    }

                }
            }

            return false;

        }

        /// <summary>
        /// Основная функция которая подбирает пароли.
        /// </summary>
        /// <param name="start"> Начальная последовательность</param>
        /// <param name="stop">  Конечная последовательность</param>
        /// <param name="hash">  Развретка </param>
        private static void BruteForce(char[] start, char[] stop, string hash)
        {
            char[] hash_arr = StringToCharArr(hash);
            int lenStart = start.Length;
            int lenStop = stop.Length;
            bool a = false;

            if (lenStart == 2)
            {
                if (lenStart != lenStop)
                {
                    char[] stop_buff = { '0', '0' };
                    a = BruteForceOnTwoSymbol(start, stop_buff);
                    ++lenStart;
                    if (lenStart != lenStop)
                    {
                        char[] start_buff = { 'A', 'A', 'A' };
                        char[] stop_buff_2 = { '0', '0', '0' };
                        a = BruteForceOnThreeSymbol(start_buff, stop_buff_2);
                        ++lenStart; //4
                        if (lenStart != lenStop)
                        {
                            char[] start_buff_2 = { 'A', 'A', 'A', 'A' };
                            char[] stop_buff_3 = { '0', '0', '0', '0' };
                            a = BruteForceOnFourSymbol(start_buff_2, stop_buff_3);
                            ++lenStart; //5
                            if (lenStart != lenStop)
                            {
                                char[] start_buff_3 = { 'A', 'A', 'A', 'A', 'A' };
                                char[] stop_buff_4 = { '0', '0', '0', '0', '0' };
                                char[] start_buff_4 = { 'A', 'A', 'A', 'A', 'A', 'A' };
                                a = BruteForceOnFiveSymbol(start_buff_3, stop_buff_4);
                                if (a == false)
                                {
                                    ++lenStart;
                                    a = BruteForceOnSexSymbol(start_buff_4, stop);
                                }
                                else
                                    Console.WriteLine("Pass NO find");
                            }
                            else
                            {
                                char[] start_buff_ = { 'A', 'A', 'A', 'A', 'A' };
                                a = BruteForceOnFiveSymbol(start_buff, stop);
                            }
                        }
                        else
                        {
                            char[] start_buff_ = { 'A', 'A', 'A', 'A' };
                            a = BruteForceOnFourSymbol(start_buff_, stop);
                        }
                    }
                    else
                    {
                        char[] start_buff = { 'A', 'A', 'A' };
                        a = BruteForceOnThreeSymbol(start_buff, stop);
                    }
                }
                else
                    a = BruteForceOnTwoSymbol(start, stop);

            }

            if (lenStart == 3)
            {
                if (lenStart != lenStop)
                {
                    char[] stop_buff = { '0', '0', '0' };
                    a = BruteForceOnThreeSymbol(start, stop_buff);
                    ++lenStart;
                    if (lenStart != lenStop)
                    {
                        char[] start_buff = { 'A', 'A', 'A', 'A' };
                        char[] stop_buff_2 = { '0', '0', '0', '0' };
                        a = BruteForceOnFourSymbol(start_buff, stop_buff_2);
                        ++lenStart;
                        if (lenStart != lenStop)
                        {
                            char[] start_buff_2 = { 'A', 'A', 'A', 'A', 'A' };
                            char[] stop_buff_3 = { '0', '0', '0', '0', '0' };
                            char[] start_buff_4 = { 'A', 'A', 'A', 'A', 'A', 'A' };
                            a = BruteForceOnFiveSymbol(start_buff_2, stop_buff_3);
                            ++lenStart;
                            if (a == false)
                            {
                                ++lenStart;
                                a = BruteForceOnSexSymbol(start_buff_4, stop);
                            }
                            else
                                Console.WriteLine("Pass NO find");
                        }
                        else
                        {
                            char[] start_buff_ = { 'A', 'A', 'A', 'A', 'A' };
                            a = BruteForceOnFiveSymbol(start_buff_, stop);
                        }
                    }
                    else
                    {
                        char[] start_buff = { 'A', 'A', 'A', 'A' };
                        a = BruteForceOnFourSymbol(start_buff, stop);
                    }
                }
                else
                    a = BruteForceOnThreeSymbol(start, stop);

            }

            if (lenStart == 4)
            {
                if (lenStart != lenStop)
                {
                    char[] stop_buff = { '0', '0', '0', '0' };
                    a = BruteForceOnFourSymbol(start, stop_buff);
                    ++lenStart;
                    if (lenStart != lenStop)
                    {
                        char[] start_buff = { 'A', 'A', 'A', 'A', 'A' };
                        char[] stop_buff_2 = { '0', '0', '0', '0', '0' };
                        char[] start_buff_4 = { 'A', 'A', 'A', 'A', 'A', 'A' };
                        a = BruteForceOnFiveSymbol(start_buff, stop_buff_2);
                        ++lenStart;
                        if (a == false)
                        {
                            ++lenStart;
                            a = BruteForceOnSexSymbol(start_buff_4, stop);
                        }
                        else
                            Console.WriteLine("Pass NO find");
                    }

                    else
                    {
                        char[] start_buff_ = { 'A', 'A', 'A', 'A', 'A' };
                        a = BruteForceOnFiveSymbol(start_buff_, stop);
                    }
                }

                else
                    a = BruteForceOnFourSymbol(start, stop);

            }
            if (lenStart == 5)
            {
                if (lenStart != lenStop)
                {

                    char[] start_buff = { 'A', 'A', 'A', 'A', 'A' };
                    char[] stop_buff_2 = { '0', '0', '0', '0', '0' };
                    char[] start_buff_4 = { 'A', 'A', 'A', 'A', 'A', 'A' };
                    a = BruteForceOnFiveSymbol(start_buff, stop_buff_2);
                    ++lenStart;
                    if (a == false)
                    {
                        ++lenStart;
                        a = BruteForceOnSexSymbol(start_buff_4, stop);
                    }
                    else
                        Console.WriteLine("Pass NO find");
                }
                else
                    a = BruteForceOnFiveSymbol(start, stop);
            }
            a = BruteForceOnSexSymbol(start, stop);
        }
    }
}

