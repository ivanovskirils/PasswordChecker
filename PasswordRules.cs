using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PasswordChecker
{
    public class PasswordRules
    {
        public bool isValid = false;
        private bool ContainsUpperLetter(string pass)
        {
            foreach (char c in pass)
            {
                if ((Char.IsLetter(c)) && (Char.IsUpper(c)))
                    return true;
            }
            Console.WriteLine("Password does not contain at least one Uppercase letter");
            return false;
        }

        private bool ContainsLowerLetter(string pass)
        {
            foreach (char c in pass)
            {
                if ((Char.IsLetter(c)) && (Char.IsLower(c)))
                    return true;
            }
            Console.WriteLine("Password does not contain at least one Lowercase letter");
            return false;
        }

        private bool ContainsDigit(string pass)
        {
            foreach (char c in pass)
            {
                if (Char.IsDigit(c))
                    return true;
            }
            Console.WriteLine("Password does not contain at least one digit");
            return false;
        }

        private bool ContainsSymbol(string pass) // 
        {
            string validPattern = ".!@#$%^&*";
            foreach (char c in pass)
            {
                if (validPattern.Contains(c))
                    return true;
            }
            Console.WriteLine("Password does not contain a special symbol");
            return false;
        }

        private bool ContainsTenCharacters(string pass)
        {
            if (pass.Length >= 10)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Password does not contain at least 10 characters");
                return false;
            }
        }


        public bool CallTheAllCheckFunctions(string pass)
        {
            if (!ContainsTenCharacters(pass))
            {
                return false;
            }
            if (!ContainsSymbol(pass))
            {
                return false;
            }
            if (!ContainsDigit(pass))
            {
                return false;
            }
            if (!ContainsLowerLetter(pass))
            {
                return false;
            }
            if (!ContainsUpperLetter(pass))
            {
                return false;
            }

            return true;
        }

        public void CheckPasswordByApi(string pass)
        {
            string sHash = string.Empty;

            using (SHA1Managed sha1 = new SHA1Managed()) // use the class to hash the password 
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(pass)); // create the hash as a bit array
                sHash = BitConverter.ToString(hash).Replace("-", ""); // convert bit array to hexadecimal
            }

            string existingPasswords = requestThroughApi(sHash.Substring(0, 5)); //call the function for API request
            if (!string.IsNullOrWhiteSpace(existingPasswords)) // if string returned, we will check our password
            {
                var passwordAr = existingPasswords.Split("\r\n", StringSplitOptions.None).ToList(); //splitting the string into separate passwords

                Dictionary<string, int> passwordDic = new Dictionary<string, int>(); // create a dictionary to collect password and for quantity of how often does the password appear in API
                foreach (string pw in passwordAr)
                {
                    var tmpPw = pw.Split(':'); //split the string on password and appearing times
                    passwordDic.TryAdd(tmpPw[0], int.Parse(tmpPw[1])); //adding to dictionary
                }

                int occassions = 0;
                passwordDic.TryGetValue(sHash, out occassions); //try to find quantity of password appearing times
                if (occassions == 0) // if there are not appearing times, the password is good
                {
                    Console.WriteLine("Password is good!");
                }
            }
            else
            {
                Console.WriteLine("Your password appeared in Database, so it is no good");
            }
        }
        private static string requestThroughApi(string hash)
        {
            string result = string.Empty; //result string

            var url = "https://api.pwnedpasswords.com/range/" + hash;// where  to make a request

            var httpRequest = (HttpWebRequest)WebRequest.Create(url); // create and obj http request
            httpRequest.Method = "GET"; // choose the method, which needed to us

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse(); // try to make a request
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) // create and object StreamReader to read the result
            {
                result = streamReader.ReadToEnd(); //reading the result, that came entirely
            }

            return result;
        }
    }
}
