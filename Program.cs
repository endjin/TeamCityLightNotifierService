namespace TeamCityLightNotifierService
{
    using System.Threading;

    class Program
    { 
        static void Main(string[] args)
        {
            var lightnotifier = new LightNotifier();
            lightnotifier.UpdateLightContinuously();
        }
    }
}
