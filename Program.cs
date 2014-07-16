namespace TeamCityLightNotifierService
{
    using System.Threading;

    class Program
    { 
        static void Main(string[] args)
        {
            var lightnotifier = new LightNotifier();
            while (true)
            {
                lightnotifier.UpdateLight();
                Thread.Sleep(10000);
            }
        }
    }
}
