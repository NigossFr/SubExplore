﻿using SubExplore.ViewModels;
using Windows.UI.ApplicationSettings;

namespace SubExplore;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
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