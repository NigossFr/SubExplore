using Microsoft.Maui.Controls;
using SubExplore.ViewModels;
using System;

namespace SubExplore.Views.Main.Components {
    public class SpotPhotosComponent : ContentView
    {
        private StackLayout _mainLayout;
        private Image _mainPhotoImage;
        private Button _addMainPhotoButton;
        private Image[] _additionalPhotos;
        private Button[] _additionalPhotoButtons;

        public SpotPhotosComponent()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _mainLayout = new StackLayout
            {
                Spacing = 10,
                    Padding = new Thickness(20)
            };

            // Titre
            _mainLayout.Children.Add(new Label
            {
                    Text = "Photos du spot",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold
                });

            // Sous-titre
            _mainLayout.Children.Add(new Label
            {
                    Text = "Ajoutez jusqu'à 3 photos - Max 5MB par photo",
                    FontSize = 14,
                    TextColor = Colors.Gray
                });

            // Photo principale
            var mainPhotoFrame = new Frame
            {
                BorderColor = Colors.Blue,
                    Padding = new Thickness(10),
                    HeightRequest = 200,
                    HasShadow = false
            };

            var mainPhotoGrid = new Grid();
            _mainPhotoImage = new Image
            {
                Aspect = Aspect.AspectFill,
                    IsVisible = false
            };

            _addMainPhotoButton = new Button
            {
                Text = "Ajouter la photo principale",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
            };
            _addMainPhotoButton.Clicked += OnAddMainPhotoClicked;

            mainPhotoGrid.Children.Add(_mainPhotoImage);
            mainPhotoGrid.Children.Add(_addMainPhotoButton);
            mainPhotoFrame.Content = mainPhotoGrid;
            _mainLayout.Children.Add(mainPhotoFrame);

            // Photos additionnelles
            var additionalPhotosGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            },
            ColumnSpacing = 10
        };

        _additionalPhotos = new Image[2];
        _additionalPhotoButtons = new Button[2];

        for (int i = 0; i < 2; i++)
        {
            var frame = new Frame
            {
                BorderColor = Colors.Blue,
                    Padding = new Thickness(5),
                    HeightRequest = 100,
                    HasShadow = false
            };

            var grid = new Grid();
            _additionalPhotos[i] = new Image
            {
                Aspect = Aspect.AspectFill,
                    IsVisible = false
            };

            _additionalPhotoButtons[i] = new Button
            {
                Text = "+",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
            };

                int index = i; // Capture pour le lambda
            _additionalPhotoButtons[i].Clicked += (s, e) => OnAddAdditionalPhotoClicked(index);

            grid.Children.Add(_additionalPhotos[i]);
            grid.Children.Add(_additionalPhotoButtons[i]);
            frame.Content = grid;

            additionalPhotosGrid.Add(frame, i, 0);
        }

        _mainLayout.Children.Add(additionalPhotosGrid);

        // Message d'aide
        _mainLayout.Children.Add(new Label
            {
                Text = "Format accepté : JPG, PNG - Max 5MB par photo",
                FontSize = 12,
                TextColor = Colors.Gray,
                Margin = new Thickness(0, 10, 0, 0)
            });

        Content = _mainLayout;
    }

        private async void OnAddMainPhotoClicked(object sender, EventArgs e)
    {
        try {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Sélectionner une photo"
                });

            if (result != null) {
                _mainPhotoImage.Source = ImageSource.FromFile(result.FullPath);
                _mainPhotoImage.IsVisible = true;
                _addMainPhotoButton.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Erreur",
                "Impossible de charger la photo", "OK");
        }
    }

        private async void OnAddAdditionalPhotoClicked(int index)
    {
        try {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Sélectionner une photo"
                });

            if (result != null) {
                _additionalPhotos[index].Source = ImageSource.FromFile(result.FullPath);
                _additionalPhotos[index].IsVisible = true;
                _additionalPhotoButtons[index].IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Erreur",
                "Impossible de charger la photo", "OK");
        }
    }
}
}