using DR_Amin_Mobile.Settings;

namespace DR_Amin_Mobile.Pages
{
    public class ApiSettingsPage : ContentPage
    {
        private readonly Entry _apiBaseUrlEntry;
        private readonly Label _resultLabel;

        public ApiSettingsPage()
        {
            Title = "API Settings";
            BackgroundColor = (Color)Application.Current!.Resources["Gray950"];

            _apiBaseUrlEntry = new Entry
            {
                Placeholder = "https://127.0.0.1:7201",
                Keyboard = Keyboard.Url,
                Text = AppSettings.GetApiBaseUrl(),
                TextColor = (Color)Application.Current.Resources["White"],
                PlaceholderColor = (Color)Application.Current.Resources["Gray400"],
                BackgroundColor = (Color)Application.Current.Resources["OffBlack"],
                Margin = new Thickness(0, 12, 0, 0)
            };

            _resultLabel = new Label
            {
                IsVisible = false,
                TextColor = (Color)Application.Current.Resources["Gray200"]
            };

            var saveButton = new Button
            {
                Text = "Save",
                Margin = new Thickness(0, 8, 0, 0)
            };
            saveButton.Clicked += OnSaveClicked;

            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = new Thickness(24, 40, 24, 24),
                    Spacing = 16,
                    Children =
                    {
                        new Label
                        {
                            Text = "API Base URL",
                            FontSize = 24,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = (Color)Application.Current.Resources["White"]
                        },
                        new Label
                        {
                            Text = "Set the default API URL used by the mobile app.",
                            TextColor = (Color)Application.Current.Resources["Gray200"]
                        },
                        _apiBaseUrlEntry,
                        saveButton,
                        _resultLabel
                    }
                }
            };
        }

        private async void OnSaveClicked(object? sender, EventArgs e)
        {
            var url = _apiBaseUrlEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var parsedUri))
            {
                _resultLabel.Text = "Please enter a valid absolute URL.";
                _resultLabel.IsVisible = true;
                return;
            }

            if (parsedUri.Scheme != Uri.UriSchemeHttp && parsedUri.Scheme != Uri.UriSchemeHttps)
            {
                _resultLabel.Text = "URL must start with http:// or https://.";
                _resultLabel.IsVisible = true;
                return;
            }

            AppSettings.SetApiBaseUrl(url);
            _resultLabel.Text = "API URL saved.";
            _resultLabel.IsVisible = true;

            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
