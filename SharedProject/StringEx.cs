﻿using System;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;

namespace SharedProject
{
    public static class StringEx
    {
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string Truncate(this string value, int maxLength, string suffix = "")
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + suffix;
        }

        // Increment the numeric suffix of an alphanumeric string

        public static string Increment(this string value, int increment = 1)
        {
            var prefix = Regex.Match(value, "^\\D+").Value;
            var number = Regex.Replace(value, "^\\D+", "");
            var i = int.Parse(number) + increment;
            return (prefix + i.ToString(new string('0', number.Length)));
        }

        public static int ToInt(this string value, int defaultIntValue = 0)
        {
            int parsedInt;

            try
            {
                parsedInt = (int)Convert.ToDouble(value);
                return parsedInt;
            }
            catch (Exception)
            {
            }

            if (int.TryParse(value, out parsedInt))
            {
                return parsedInt;
            }

            return defaultIntValue;
        }

        public static bool IsInt(this string value)
        {
            int i = 0;
            bool result = int.TryParse(value, out i);
            return result;
        }

        public static double ToDouble(this string value, int defaultValue = 0)
        {
            double parsedDouble;

            try
            {
                parsedDouble = Convert.ToDouble(value);
                return parsedDouble;
            }
            catch (Exception)
            {
            }

            return defaultValue;
        }

        public static String ToCompactString(this string value)
        {
            double num;

            if (double.TryParse(value, out num))

                // It's a number!
                //return String.Format("{0:G29}", num);
                return num.ToString("0.#########################");

            return value;
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static string ReplaceIgnoreCase(this string source, string old_substring, string new_substring)
        {
            return Regex.Replace(source, old_substring, new_substring, RegexOptions.IgnoreCase);
        }

        public static string GetStringFromConsole(String displayMessage)
        {
            Console.Write(displayMessage);
            var result = Console.ReadLine();
            //Console.WriteLine("");
            return result;
        }

        public static SecureString GetSecureStringFromConsole(String displayMessage)
        {
            SecureString pass = new SecureString();
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass.RemoveAt(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }

        public static string SecureStringToString(SecureString secureString)
        {
            string password = new System.Net.NetworkCredential(string.Empty, secureString).Password;
            return password;
        }

    }
}
