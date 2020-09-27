using Xamarin.Forms;

[assembly: Dependency(typeof(Digyl.iOS.Services.LocationService))]


namespace Digyl.iOS.Services
{
    public class LocationService : ILocationService
    {
        public bool IsLocationServiceOn()
        {
            return false;
        }

        public void StartLocationService()
        {

        }

        public void StopLocationService()
        {

        }

        public void UpdateNotification(string title, string text, int id)
        {
            //throw new System.NotImplementedException();
        }
    }
}