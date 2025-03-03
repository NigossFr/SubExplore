using SubExplore.ViewModels.Main;

namespace SubExplore.Views.Main;

public partial class StructureDetailsPage : ContentPage
{
    public StructureDetailsPage(StructureDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}