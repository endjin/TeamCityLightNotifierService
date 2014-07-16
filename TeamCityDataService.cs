namespace TeamCityLightNotifierService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using TeamCitySharp;
    using TeamCitySharp.DomainEntities;

    public class TeamCityDataService
    {
        private readonly TeamCityClient client;

        public TeamCityDataService()
        {
            var server = GetServerFromUser();
            this.client = new TeamCityClient(server);

            var username = GetUsernameFromUser();
            var password = GetPasswordFromUser();
            this.client.Connect(username, password);
        }
        private static string GetServerFromUser()
        {
            Console.Write("Enter Server name: ");
            var server = Console.ReadLine();
            return server;
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

        public List<string> GetBuildIds(string Filename)
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

        public List<Build> GetTeamCityBuilds(List<string> buildids)
        {
            // get all BuildConfig ids
            var buildConfigIds = this.client.BuildConfigs.All()
                                       .Select(allBuildConfigs => allBuildConfigs.Id);

            // compare builds ids to subscribe to, with list of all valid ids, and only take the valid ones.
            var validBuildIds = buildids.Intersect(buildConfigIds);

            // get all Last builds using BuildConfig ids
            var lastBuilds = new List<Build>();

            // for each valid build id, get the last build, and add to a new list
            foreach (var id in validBuildIds)
            {
                var lastbuild = this.client.Builds.LastBuildByBuildConfigId(id);
                lastBuilds.Add(lastbuild);
            }
            return lastBuilds;
        }
    }
}