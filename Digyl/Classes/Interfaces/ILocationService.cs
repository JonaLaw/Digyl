public interface ILocationService
{
    void UpdateNotification(string title, string text, int id);
    bool IsLocationServiceOn();
    void StartLocationService();

    void StopLocationService();
}
