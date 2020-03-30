using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace APINancy_Quete2
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new UserContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                List<User> userList = new List<User>();
                Random random = new Random();
                for (int i = 1; i <= 20; i++)
                {
                    var user = new User()
                    {
                        UserId = i,
                        Firstname = "UserName" + i,
                        Password = "Password" + random.Next(1, 200)
                    };
                    userList.Add(user);
                }

                context.AddRange(userList);
                context.SaveChanges();
            }

            HostConfiguration hostConfiguration = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true },
            };
            using (var host = new NancyHost(hostConfiguration, new Uri("http://localhost:1234")))
            {
                host.Start();
                Console.WriteLine("Running on http://localhost:1234");
                Console.ReadLine();
                host.Stop();
            }
        }
    }
}
