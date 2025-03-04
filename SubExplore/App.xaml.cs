using SubExplore.ViewModels;
#if WINDOWS
using Windows.UI.ApplicationSettings;
#endif
namespace SubExplore;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        MainPage = serviceProvider.GetRequiredService<AppShell>();
    }

    protected override void OnStart()
    {
        base.OnStart();
        // Code à exécuter au démarrage de l'application si nécessaire
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        // Code à exécuter lorsque l'application passe en arrière-plan
    }

    protected override void OnResume()
    {
        base.OnResume();
        // Code à exécuter lorsque l'application reprend
    }
}