using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MultilevelMenuExample
{
    public class Storage
    {
        private static Storage _storage;
        private static Wallet activeWallet { get; set; } = null;
        private static string FilePath;
        private List<User> Users { get; }
        public User ActiveUser { get; private set; }


        public static Storage GetInstance()
        {
            if (_storage == null)
            {
                _storage = new Storage();
            }

            return _storage;
        }
        

        public Storage()
        {
           
          string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;              // here we get the current working directory of the application
            FilePath = Path.Combine(currentDirectory, "Users.json");                  // you can find the json file in bin\Debug\net6.0
            Users = LoadUsersFromFile();                                                                      //  Console.WriteLine($"Current working directory: {currentDirectory}");

        }


        public List<User> GetUsers()
        {
            return Users;
        }


        private List<User> LoadUsersFromFile()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }

            CreateEmptyFile();
            return new List<User>();
        }


        public void SaveUsersToFile()
        {
            var json = JsonConvert.SerializeObject(Users);
            File.WriteAllText(FilePath, json);
        }


        private void CreateEmptyFile()
        {
            File.WriteAllText(FilePath, "[]");
        }


        public void AddUser(User user)
        {
            if (Users.Any(x => x.Email.ToLower() == user.Email.ToLower()))
            {
                throw new ArgumentException("There are already users with the same email");
            }

            Users.Add(user);
            SaveUsersToFile();
        }



        public User FindUserByEmail(string email)
        {
            var user = Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email {email} not found!");
            }

            return user;
        }



        public void SetActiveUser(string email)
        {
            Console.WriteLine($"Setting active user: {email}");
            ActiveUser = GetUsers().FirstOrDefault(u => u.Email == email);
            SaveUsersToFile(); // Save changes to file
        }



        public static Wallet ActiveWallet
        {
            get
            {
                if (activeWallet != null)
                    return activeWallet;
                else
                {
                    throw new Exception("Wallet not chosen");
                }
            }
            set
            {
                activeWallet = value;
            }
        }



        public User GetActiveUser()
        {
            var activeUser = ActiveUser;
            return activeUser;
        }



        public void LogOut()
        {
            ActiveUser = null;
        }
    }
}
