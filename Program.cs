namespace TeamCityLightNotifierService
{
    using System.Threading;

    class Program
    { 
        static void Main(string[] args)
        {
            var lightnotifier = new LightNotifier();
            var teamcitydataservice = new TeamCityDataService();
            while (true)
            {
                var buildids = teamcitydataservice.GetBuildIds("BuildConfigIds.txt");
                var lastBuilds = teamcitydataservice.GetTeamCityBuilds(buildids);

                lightnotifier.UpdateLight(lastBuilds);
                Thread.Sleep(10000);
            }
        }
    }
}
