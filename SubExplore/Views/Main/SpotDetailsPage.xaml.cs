using SubExplore.ViewModels.Main;

namespace SubExplore.Views.Main;

public partial class SpotDetailsPage : ContentPage
{
    public SpotDetailsPage(SpotDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is SpotDetailsViewModel viewModel)
        {
            await viewModel.InitializeAsync(new Dictionary<string, object>());
        }
    }
}