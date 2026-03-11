using System.Net.Http.Json;
using DR_Amin_Mobile.Settings;

namespace DR_Amin_Mobile.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object? sender, EventArgs e)
        {
            if (!AppSettings.HasStoredApiBaseUrl())
            {
                await Shell.Current.GoToAsync("//ApiSettingsPage");
                return;
            }

            var username = UsernameEntry.Text?.Trim();
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Login", "Username and password are required.", "OK");
                return;
            }

            LoginButton.IsEnabled = false;
            ResultLabel.TextColor = (Color)Application.Current!.Resources["Gray200"];
            ResultLabel.Text = "Signing in...";
            ResultLabel.IsVisible = true;

            try
            {
                using var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(AppSettings.GetApiBaseUrl())
                };

                var request = new LoginRequestDto
                {
                    Username = username,
                    Password = password
                };

                var response = await httpClient.PostAsJsonAsync("/api/v1/Auth/login", request);

                if (!response.IsSuccessStatusCode)
                {
                    ResultLabel.Text = "Login failed. Check your credentials.";
                    return;
                }

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                if (loginResponse == null)
                {
                    ResultLabel.Text = "Login failed. Invalid server response.";
                    return;
                }

                if (loginResponse.RequiresTwoFactor)
                {
                    ResultLabel.Text = "Two-factor authentication is required.";
                    return;
                }

                await SaveAuthSessionAsync(loginResponse);

                ResultLabel.Text = "Login successful.";
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception)
            {
                ResultLabel.Text = "Unable to connect to the API.";
            }
            finally
            {
                LoginButton.IsEnabled = true;
            }
        }

        private static async Task SaveAuthSessionAsync(LoginResponseDto response)
        {
            await SecureStorage.SetAsync("auth_access_token", response.AccessToken ?? string.Empty);
            await SecureStorage.SetAsync("auth_refresh_token", response.RefreshToken ?? string.Empty);
            await SecureStorage.SetAsync("auth_username", response.Username ?? string.Empty);
            await SecureStorage.SetAsync("auth_user_id", response.UserId.ToString());
            await SecureStorage.SetAsync("auth_expires_at", response.ExpiresAt.ToString("O"));
            await SecureStorage.SetAsync("auth_roles", string.Join(',', response.Roles ?? Enumerable.Empty<string>()));
        }

        private class LoginRequestDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        private class LoginResponseDto
        {
            public int UserId { get; set; }
            public string AccessToken { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
            public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
            public bool RequiresTwoFactor { get; set; }
        }
    }
}
