using System;

namespace PasswordChecker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PasswordRules passwordRules = new PasswordRules();
            bool isValid = false;
            string pass = string.Empty;

            while (!isValid)
            {
                Console.WriteLine("Input password to check: ");

                pass = Console.ReadLine();

                isValid = passwordRules.CallTheAllCheckFunctions(pass);
            }
            
            if (isValid)
            {
                passwordRules.CheckPasswordByApi(pass);
            }
        }
    }
}
