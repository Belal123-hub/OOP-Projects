using MultilevelMenuExample.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCrypt.Net;

namespace MultilevelMenuExample
{
    public interface IBusinessLogicService
    {
        (bool, string) Register(string name,string email, string password, string birthDate);
        (bool, string,string) Login(string email, string password);

        List<Wallet> Get_Your_Wallets();

        (bool, string) ConstructWallet(string name, Money money);
        Wallet? ChooseWallet(int index);

        public List<TransactionDetails> CollectStatistics(DateTime from, DateTime to);
        (bool, string) DeleteWallet(int index);
        
    }

    public class BusinessLogicService : IBusinessLogicService
    {
        private List<Wallet> userWallets = new List<Wallet>();
        private Storage Storage { get; }

        public BusinessLogicService()
        {
            Storage = Storage.GetInstance();
        }

        public List<Wallet> Get_Your_Wallets()
        {
            var user = Storage.GetActiveUser();
            if (user == null )
            {
                return null;   
            }
            return user.Wallets;
        }

        public (bool, string) Register(string name, string email, string password, string birthDate)
        {
            try
            {
                var mail = new MailAddress(email);
            }
            catch (FormatException)
            {
                return (false, "Email is incorrect!");
            }

            var passwordScore = PasswordAdvisor.CheckStrength(password);
            if (passwordScore < PasswordScore.Medium)
            {
                return (false, "Your password is too weak!");
            }

            if (!DateTime.TryParseExact(birthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var bithDateParsed))
            {
                return (false, "DateTime has incorrect format!");
            }

            // Hash the password before storing it
            string hashedPassword = HashPassword(password);

            try
            {
                Storage.AddUser(new User
                {
                    Name = name,
                    BirthDate = bithDateParsed,
                    Email = email,
                    Password = hashedPassword  // Store the hashed password
                });
            }
            catch (ArgumentException)
            {
                return (false, "There is already a user with the same email!");
            }

            // Login
            Storage.SetActiveUser(email);

            return (true, "Successful registration! You will be automatically logged in!");
        }


        public (bool, string, string) Login(string email, string password)
        {
            try
            {
                var user = Storage.FindUserByEmail(email);

                // Check if the stored password is hashed using BCrypt
                bool isBCryptHashed = user.Password.StartsWith("$2");

                if (isBCryptHashed)
                {
                    // Compare the entered password with the BCrypt hashed password
                    bool passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);

                    if (passwordMatch)
                    {
                        Storage.SetActiveUser(email);
                        return (true, "Successful login!", user.Name);
                    }
                }
                else
                {
                    if (password == user.Password)
                    {
                        Storage.SetActiveUser(email);
                        return (true, "Successful login!", user.Name);
                    }
                }

                return (false, "Password is incorrect!", string.Empty);
            }
            catch (KeyNotFoundException)
            {
                return (false, "User with provided email is not found!", string.Empty);
            }
        }


        private string HashPassword(string password)
        {                                                           //Here we generate a salt and hash the password using BCrypt
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);      // here we can adjust the salt factor as needed  
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hashedPassword;
        }

        public Wallet? ChooseWallet(int index)
        {
            try
            {
                var user = Storage.GetActiveUser();
                if (user == null)
                {
                    //return (false, " No active user");
                    return null;
                }

                user.ActiveWallet = user.Wallets[index];
                //return (true, " wallet selected");
                return user.ActiveWallet;
            }

            catch (Exception ex)
            {
                throw new Exception("nvalid wallet");
            }
        }

        public (bool, string) ConstructWallet(string name, Money money)
        {
            try
            {
                var user = Storage.GetActiveUser();

                if (user == null)                          // Check if the user is null before trying to access its properties
                {
                    return (false, "User is not logged in!");
                }

                if (user.Wallets == null)                    // Ensure that the user has a valid list of wallets before adding a new one
                {
                    user.Wallets = new List<Wallet>();
                }

                user.Wallets.Add(new Wallet(name, money, money._currency));
                return (true, "Successfully created wallet!");
            }
            catch (KeyNotFoundException)
            {
                return (false, "Failed to create wallet!");
            }
        }

        public (bool, string) DeleteWallet(int index)
        {
            try
            {
                var user = Storage.GetActiveUser();
                if (user == null)
                {
                    return (false, "No active user");
                }

                if (index < 0 || index >= user.Wallets.Count)
                {
                    return (false, "Invalid wallet index");
                }

                user.Wallets.RemoveAt(index);

                return (true, "Wallet deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, "Failed to delete wallet");
            }
        }


        public List<TransactionDetails> CollectStatistics(DateTime from, DateTime to)
        {
            var activeUser = Storage.GetActiveUser();  

            if (activeUser == null)
            {
                Console.WriteLine("Debug: No active user!");
                return new List<TransactionDetails> { new TransactionDetails { Amount = Money.Zero, Date = DateTime.MinValue, Type = "", Category = "No active user!" } };
            }

            if (activeUser.ActiveWallet == null)
            {
                Console.WriteLine("Debug: No active wallet selected!");
                return new List<TransactionDetails> { new TransactionDetails { Amount = Money.Zero, Date = DateTime.MinValue, Type = "", Category = "No active wallet selected!" } };
            }

            Console.WriteLine($" Active User is : {activeUser.Name}, Active Wallet is : {activeUser.ActiveWallet.Name}");

            try
            {
                var statistics = activeUser.ActiveWallet.CheckStatistics(from, to);

                if (statistics != null && statistics.Count > 0)
                {
                    return statistics;
                }

                else
                {
                    Console.WriteLine("Debug: No transactions found for the specified period.");
                    return new List<TransactionDetails> { new TransactionDetails { Amount = Money.Zero, Date = DateTime.MinValue, Type = "", Category = "No transactions found for the specified period." } };
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Debug: Exception: {ex.Message}");
                return new List<TransactionDetails> { new TransactionDetails { Amount = Money.Zero, Date = DateTime.MinValue, Type = "", Category = "An error occurred: " + ex.Message } };
            }
        }



    }

    public enum PasswordScore
    {
        Blank = 0,
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }

    public class PasswordAdvisor
    {
        public static PasswordScore CheckStrength(string password)
        {
            int score = 0;

            if (password.Length < 1)
                return PasswordScore.Blank;

            if (password.Any(char.IsUpper))
                score++;

            if (password.Any(char.IsLower))
                score++;

            if (password.Any(char.IsDigit))
                score++;

            if (password.Any(c => !char.IsLetterOrDigit(c)))
                score++; 

            return (PasswordScore)score;
        }
    }

}
