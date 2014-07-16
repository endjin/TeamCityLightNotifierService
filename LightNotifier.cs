namespace TeamCityLightNotifierService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Plenom.Components.Busylight.Sdk;

    using TeamCitySharp.DomainEntities;
    using TeamCitySharp.Locators;

    class LightNotifier
    {
        private readonly BusylightLyncController lightController;
        private readonly TeamCityDataService teamcitydataservice;

        public LightNotifier()
        {
            this.teamcitydataservice = new TeamCityDataService();
            lightController = new BusylightLyncController();
        }

        public void UpdateLightContinuously()
        {
            while (true)
            {
                UpdateLight();
                Thread.Sleep(10000);
            }
        }

        private void FlashLightWhenError()
        {
            for (int i = 0; i < 20; i++)
            {
                lightController.Light(BusylightColor.Red);
                Thread.Sleep(500);
                this.GetPinkBusyLightColor();
                Thread.Sleep(500);

            }

        }
        private void GetPinkBusyLightColor()
        {
            lightController.Light(new BusylightColor
            {
                BlueRgbValue = 255,
                RedRgbValue = 255,
                GreenRgbValue = 0
            });
        }

        private void UpdateLight()
        {
            var buildids = teamcitydataservice.GetBuildIds("BuildConfigIds.txt");
            var lastBuilds = teamcitydataservice.GetTeamCityBuilds(buildids);

            if (lastBuilds.All(build => build.Status == BuildStatus.SUCCESS.ToString()))
            {
                lightController.Light(BusylightColor.Green);
            }
            else if (lastBuilds.Any(build => build.Status == BuildStatus.FAILURE.ToString()))
            {
                this.FlashLightWhenError();


            }
            else if (lastBuilds.Any(build => build.Status == BuildStatus.ERROR.ToString()))
            {
                this.GetPinkBusyLightColor();
            }
        }
    }
}