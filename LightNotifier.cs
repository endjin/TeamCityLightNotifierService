namespace TeamCityLightNotifierService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Plenom.Components.Busylight.Sdk;

    using TeamCitySharp.DomainEntities;
    using TeamCitySharp.Locators;

    class LightNotifier {
        public LightNotifier()
        {
            
        }
        private void FlashLightWhenError(BusylightLyncController lightController)
        {
            for (int i = 0; i < 20; i++)
            {
                lightController.Light(BusylightColor.Red);
                Thread.Sleep(500);
                this.GetPinkBusyLightColor(lightController);
                Thread.Sleep(500);

            }

        }
        private void GetPinkBusyLightColor(BusylightLyncController lightController)
        {
            lightController.Light(new BusylightColor()
            {
                BlueRgbValue = 255,
                RedRgbValue = 255,
                GreenRgbValue = 0
            });
        }
        public void UpdateLight(List<Build> lastBuilds)
        {
            var lightController = new BusylightLyncController();

            if (lastBuilds.All(build => build.Status == BuildStatus.SUCCESS.ToString()))
            {
                lightController.Light(BusylightColor.Green);
            }
            else if (lastBuilds.Any(build => build.Status == BuildStatus.FAILURE.ToString()))
            {
                this.FlashLightWhenError(lightController);


            }
            else if (lastBuilds.Any(build => build.Status == BuildStatus.ERROR.ToString()))
            {
                this.GetPinkBusyLightColor(lightController);
            }
        }
    }
}