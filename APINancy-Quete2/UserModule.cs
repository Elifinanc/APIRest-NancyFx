using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APINancy_Quete2
{
    public class UserModule:NancyModule
    {
        public new UserContext Context { get; set; }

        public UserModule()
        {
            Context = new UserContext();

            Get("/", parameters => ReturnAllUsers());

            // Get JSON about a given user
            Get("/users/{UserId:int}", parameters => ReturnUserData(parameters.UserId));

            // Delete an existing user            
            Delete("/users/delete/{UserId:int}", parameters => DeleteUser(parameters.UserId));

            // Add a new user
            Put("/users/new/{Fistname:string}/{Lastname:string}/{Password:string}", parameters => PutNewUser(parameters.Firstname, parameters.Lastname, parameters.Password));

            // Authentify a user
            Post("/authentify/{Fistname:string}/{Password:string}", parameters => AuthentifyUser(parameters.Firstname, parameters.Password));
        }

        public string ReturnAllUsers()
        {
            using (var context = new UserContext())
            {
                var allUsers = from u in context.User
                               select u;

                string output = JsonConvert.SerializeObject(allUsers);
                return output;
            }
        }

        public string ReturnUserData(int userId)
        {
            using (var context = new UserContext())
            {
                var selectedUser = (from u in context.User
                                    where u.UserId == userId
                                    select u).FirstOrDefault();

                string output = JsonConvert.SerializeObject(selectedUser);
                return output;
            }
        }

        public void DeleteUser(int userId)
        {
            using (var context = new UserContext())
            {
                var selectedUser = (from u in context.User
                                    where u.UserId == userId
                                    select u).FirstOrDefault();

                context.User.Remove(selectedUser);
                context.SaveChanges();
            }
        }

        public string PutNewUser(dynamic FirstName, dynamic LastName, dynamic Password)
        {
            using (var context = new UserContext())
            {
                var allUsers = (from u in context.User
                                select u).ToList();

                int NewUserId = allUsers.Count() + 1;

                User newUser = new User()
                {
                    UserId = NewUserId,
                    Firstname = FirstName,
                    Lastname = LastName,
                    Password = Password
                };

                context.Add(newUser);
                context.SaveChanges();

                var output = JsonConvert.SerializeObject(newUser);
                return output;
            }
        }

        public string AuthentifyUser(dynamic FirstName, dynamic Password)
        {
            using (var context = new UserContext())
            {
                var allUsers = (from u in context.User
                                select u).ToList();

                List<string> usersName = new List<string>();
                foreach (User u in allUsers)
                {
                    usersName.Add(u.Firstname);
                }

                JsonMessage Output = new JsonMessage();

                string firstName = Convert.ToString(FirstName);

                if (usersName.Contains(firstName))
                {
                    var selectedUser = (from u in context.User
                                        where u.Firstname == firstName
                                        select u).FirstOrDefault();

                    if (selectedUser.Password == Password)
                    {
                        Output.Message = "Ok, you have entered the correct password";
                    }
                    else
                    {
                        Output.Message = "You have entered an incorrect password, try again later!";
                    }
                }
                else
                {
                    Output.Message = "You are not a registered user, we can not authenticate you";
                }
                string output = JsonConvert.SerializeObject(Output.Message);
                return output;
            }
        }
    }
}
