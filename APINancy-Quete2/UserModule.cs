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
            Get("/users/delete/{UserId:int}", parameters => DeleteUser(parameters.UserId));

            // Add a new user
            Get("/new/{Firstname}/{Password}", parameters => PutNewUser(parameters.Firstname, parameters.Password));

            // Authentify a user
            Get("/authentify/{Firstname}/{Password}", parameters => AuthentifyUser(parameters.Firstname, parameters.Password));
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

        public string DeleteUser(int userId)
        {
            using (var context = new UserContext())
            {
                JsonMessage Output = new JsonMessage();

                var selectedUser = (from u in context.User
                                    where u.UserId == userId
                                    select u).FirstOrDefault();

                Output.Message = "Delete user :" + selectedUser.Firstname;

                context.User.Remove(selectedUser);

                //je réactualise les UserId sachant qu'un à été supprimé, pour garder la continuité des nombres en UserId
                /*var allRemainingUsersWithBiggerId = from u in context.User
                                        where u.UserId > userId
                                        select u;
                foreach (User user in allRemainingUsersWithBiggerId)
                {
                    context.User.Remove(user);
                    user.UserId = user.UserId - 1;
                }

                context.AddRange(allRemainingUsersWithBiggerId);*/

                context.SaveChanges();

                string output = JsonConvert.SerializeObject(Output.Message);
                return output;
            }
        }

        public string PutNewUser(object Firstname, object Password)
        {
            using (var context = new UserContext())
            {
                var allUsers = (from u in context.User
                                select u).ToList();

                int NewUserId = allUsers.Count() + 1;

                User newUser = new User()
                {
                    UserId = NewUserId,
                    Firstname = Firstname.ToString(),
                    Password = Password.ToString()
                };

                context.Add(newUser);
                context.SaveChanges();

                var output = JsonConvert.SerializeObject(newUser);
                return output;
            }
        }

        public string AuthentifyUser(object Firstname, object Password)
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

                string firstname = Firstname.ToString();

                if (usersName.Contains(firstname))
                {
                    var selectedUser = (from u in context.User
                                        where u.Firstname == firstname
                                        select u).FirstOrDefault();

                    if (selectedUser.Password == Password.ToString())
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
