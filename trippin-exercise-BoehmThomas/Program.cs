using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace trippin_exercise_BoehmThomas
{
    class Program
    {
        static readonly HttpClient HttpClient = new HttpClient() { BaseAddress = new Uri("https://services.odata.org/TripPinRESTierService/(S(npj54vjthjj0d1nvunlzt1sf))/") };

        static async Task Main(string[] args)
        {
            var fileContent = await File.ReadAllTextAsync("users.json");
            IEnumerable<JSONUser> users = JsonSerializer.Deserialize<IEnumerable<JSONUser>>(fileContent);

            foreach (var user in users)
            {
                var people = await HttpClient.GetAsync("People('" + user.UserName + "')");
                if (!people.IsSuccessStatusCode)
                {
                    //User not found
                    var newUser = new StringContent(JsonSerializer.Serialize(new TrippingUser(user)), Encoding.UTF8, "application/json");
                    var result = await HttpClient.PostAsync("People", newUser);
                    try
                    {
                        result.EnsureSuccessStatusCode();
                        Console.WriteLine("User " + user.UserName + " was created!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error when trying to create user!");
                    }
                }
                else
                {
                    Console.WriteLine("User " + user.UserName + " already exists!");
                }
            }
        }
    }
}
