using SubExplore.ViewModels.Spot;

namespace SubExplore.Views.Spot;

public partial class AddSpotPage : ContentPage
{
    private readonly AddSpotViewModel _viewModel;

    public AddSpotPage(AddSpotViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync(new Dictionary<string, object>());
    }

    protected override bool OnBackButtonPressed()
    {
        if (_viewModel.HasUnsavedChanges)
        {
            ShowDiscardChangesAlert();
            return true;
        }
        return base.OnBackButtonPressed();
    }

    private async void ShowDiscardChangesAlert()
    {
        bool discard = await DisplayAlert(
            "Modifications non enregistrées",
            "Voulez-vous abandonner vos modifications ?",
            "Oui",
            "Non"
        );

        if (discard)
        {
            await Navigation.PopAsync();
        }
    }
}