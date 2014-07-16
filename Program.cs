namespace TeamCityLightNotifierService
{

    class Program
    { 
        static void Main(string[] args)
        {
            var lightnotifier = new LightNotifier();
            lightnotifier.UpdateLightContinuously();
        }
    }
}
