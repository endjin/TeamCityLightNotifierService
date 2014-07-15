namespace TeamCityLightNotifierService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Threading;

    using Plenom.Components.Busylight.Sdk;

    using TeamCitySharp;
    using TeamCitySharp.DomainEntities;
    using TeamCitySharp.Locators;

    class Program
    {
        static void Main(string[] args)
        {   
            var server = GetServerFromUser();
            var client = new TeamCityClient(server);

            var username = GetUsernameFromUser();
            var password = GetPasswordFromUser();
            client.Connect(username,password);


            while (true)
            {
                var buildids = GetBuildIds("BuildConfigIds.txt");
                var lastBuilds = GetTeamCityBuilds(client, buildids);

                UpdateLight(lastBuilds);
                Thread.Sleep(10000);
            }

        }

        private static string GetUsernameFromUser()
        {
           Console.Write("Enter Username: ");
            var username = Console.ReadLine();
            return username;
        }

        private static string GetPasswordFromUser()
        {
            string pass = "";
            Console.Write("Enter your password: ");
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return pass;
        }

        private static string GetServerFromUser()
        {
        Console.Write("Enter Server name: ");
            var server = Console.ReadLine();
            return server;
        }

        private static List<string> GetBuildIds(string Filename)
        {
            var buildids = new List<string>();
            using (StreamReader reader = new StreamReader(Filename))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    buildids.Add(line);
                }
            }
            return buildids;
        }

        private static List<Build> GetTeamCityBuilds(TeamCityClient client, List<string> buildids)
        {
            // get all BuildConfig ids
            var buildConfigIds = client.BuildConfigs.All()
                                       .Select(allBuildConfigs => allBuildConfigs.Id);

            // compare builds ids to subscribe to, with list of all valid ids, and only take the valid ones.
            var validBuildIds = buildids.Intersect(buildConfigIds);

            // get all Last builds using BuildConfig ids
            var lastBuilds = new List<Build>();

            // for each valid build id, get the last build, and add to a new list
            foreach (var id in validBuildIds)
            {
                var lastbuild = client.Builds.LastBuildByBuildConfigId(id);
                lastBuilds.Add(lastbuild);
            }
            return lastBuilds;
        }

        private static void UpdateLight(List<Build> lastBuilds)
        {
            var lightController = new BusylightLyncController();

            if (lastBuilds.All(build => build.Status == BuildStatus.SUCCESS.ToString()))
            {
                lightController.Light(BusylightColor.Green);
            }
            else if (lastBuilds.Any(build => build.Status == BuildStatus.FAILURE.ToString()))
            {
                FlashLightWhenError(lightController);
               
                    
            }
            else if (lastBuilds.Any(build => build.Status == BuildStatus.ERROR.ToString()))
            {
                GetPinkBusyLightColor(lightController);
            }
        }

        private static void FlashLightWhenError(BusylightLyncController lightController)
        {
            for (int i = 0; i < 20; i++)
            {
                lightController.Light(BusylightColor.Red);
                Thread.Sleep(500);
                GetPinkBusyLightColor(lightController);
                Thread.Sleep(500);
                
            }
            
        }

        private static void GetPinkBusyLightColor(BusylightLyncController lightController)
        {
            lightController.Light(new BusylightColor()
            {
                BlueRgbValue = 255,
                RedRgbValue = 255,
                GreenRgbValue = 0
            });
        }
    }
}
