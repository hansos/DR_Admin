using System.Net.Http.Headers;
using System.Text.Json;
using DR_Amin_Mobile.Settings;

namespace DR_Amin_Mobile.Pages
{
    public class StatusPage : ContentPage
    {
        private readonly Label _messageLabel;
        private readonly Button _refreshButton;

        private readonly Label _domainsTodayLabel;
        private readonly Label _domainsLastWeekLabel;
        private readonly Label _domainsLastMonthLabel;
        private readonly Label _domainsTotalLabel;

        private readonly Label _customersTodayLabel;
        private readonly Label _customersLastWeekLabel;
        private readonly Label _customersLastMonthLabel;
        private readonly Label _customersTotalLabel;

        private bool _loaded;

        public StatusPage()
        {
            Title = "Status";
            BackgroundColor = (Color)Application.Current!.Resources["Gray950"];

            _refreshButton = new Button
            {
                Text = "Refresh"
            };
            _refreshButton.Clicked += OnRefreshClicked;

            _messageLabel = new Label
            {
                IsVisible = false,
                TextColor = (Color)Application.Current.Resources["Gray200"]
            };

            _domainsTodayLabel = CreateValueLabel();
            _domainsLastWeekLabel = CreateValueLabel();
            _domainsLastMonthLabel = CreateValueLabel();
            _domainsTotalLabel = CreateValueLabel();

            _customersTodayLabel = CreateValueLabel();
            _customersLastWeekLabel = CreateValueLabel();
            _customersLastMonthLabel = CreateValueLabel();
            _customersTotalLabel = CreateValueLabel();

            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = new Thickness(24, 20, 24, 24),
                    Spacing = 16,
                    Children =
                    {
                        new Label
                        {
                            Text = "Status overview",
                            FontSize = 26,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = (Color)Application.Current.Resources["White"]
                        },
                        _refreshButton,
                        _messageLabel,
                        CreateStatsSection("Domains", _domainsTodayLabel, _domainsLastWeekLabel, _domainsLastMonthLabel, _domainsTotalLabel),
                        CreateStatsSection("Customers", _customersTodayLabel, _customersLastWeekLabel, _customersLastMonthLabel, _customersTotalLabel)
                    }
                }
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_loaded)
            {
                return;
            }

            _loaded = true;
            await LoadStatsAsync();
        }

        private async void OnRefreshClicked(object? sender, EventArgs e)
        {
            await LoadStatsAsync();
        }

        private async Task LoadStatsAsync()
        {
            _refreshButton.IsEnabled = false;
            _messageLabel.IsVisible = false;
            SetLoadingState();

            try
            {
                if (!AppSettings.HasStoredApiBaseUrl())
                {
                    _messageLabel.Text = "Set API URL first.";
                    _messageLabel.IsVisible = true;
                    await Shell.Current.GoToAsync("//ApiSettingsPage");
                    return;
                }

                var token = await SecureStorage.GetAsync("auth_access_token");
                if (string.IsNullOrWhiteSpace(token))
                {
                    _messageLabel.Text = "Login required.";
                    _messageLabel.IsVisible = true;
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                using var client = new HttpClient
                {
                    BaseAddress = new Uri(AppSettings.GetApiBaseUrl())
                };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var domainsResult = await TryLoadCreatedDatesAsync(client, "RegisteredDomains");
                var customersResult = await TryLoadCreatedDatesAsync(client, "Customers");

                if (domainsResult.IsSuccess)
                {
                    ApplyStats(CalculateStats(domainsResult.Items), _domainsTodayLabel, _domainsLastWeekLabel, _domainsLastMonthLabel, _domainsTotalLabel);
                }
                else
                {
                    SetUnavailable(_domainsTodayLabel, _domainsLastWeekLabel, _domainsLastMonthLabel, _domainsTotalLabel);
                }

                if (customersResult.IsSuccess)
                {
                    ApplyStats(CalculateStats(customersResult.Items), _customersTodayLabel, _customersLastWeekLabel, _customersLastMonthLabel, _customersTotalLabel);
                }
                else
                {
                    SetUnavailable(_customersTodayLabel, _customersLastWeekLabel, _customersLastMonthLabel, _customersTotalLabel);
                }

                var messages = new List<string>();
                if (!domainsResult.IsSuccess)
                {
                    messages.Add(domainsResult.ErrorMessage);
                }

                if (!customersResult.IsSuccess)
                {
                    messages.Add(customersResult.ErrorMessage);
                }

                _messageLabel.Text = messages.Count == 0 ? "Updated." : string.Join(" ", messages);
                _messageLabel.IsVisible = true;
            }
            catch (Exception ex)
            {
                _messageLabel.Text = $"Failed to load status: {ex.Message}";
                _messageLabel.IsVisible = true;
                SetUnavailableState();
            }
            finally
            {
                _refreshButton.IsEnabled = true;
            }
        }

        private static async Task<LoadResult> TryLoadCreatedDatesAsync(HttpClient client, string resourcePath)
        {
            try
            {
                var items = await LoadCreatedDatesAsync(client, resourcePath);
                return LoadResult.Success(items);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return LoadResult.Fail($"No access to {resourcePath} (403).");
            }
            catch
            {
                return LoadResult.Fail($"Failed loading {resourcePath}.");
            }
        }

        private static async Task<List<DateTime>> LoadCreatedDatesAsync(HttpClient client, string resourcePath)
        {
            var all = new List<DateTime>();
            var pageNumber = 1;
            const int pageSize = 200;
            var totalPages = 1;

            while (pageNumber <= totalPages)
            {
                var response = await client.GetAsync($"/api/v1/{resourcePath}?pageNumber={pageNumber}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);

                var items = ExtractItems(doc.RootElement).ToList();
                foreach (var item in items)
                {
                    if (TryGetDate(item, out var createdAt))
                    {
                        all.Add(createdAt);
                    }
                }

                totalPages = ExtractTotalPages(doc.RootElement) ?? totalPages;
                pageNumber++;

                if (items.Count == 0)
                {
                    break;
                }
            }

            return all;
        }

        private static IEnumerable<JsonElement> ExtractItems(JsonElement root)
        {
            if (root.ValueKind == JsonValueKind.Array)
            {
                return root.EnumerateArray();
            }

            if (TryGetProperty(root, "Data", out var data) || TryGetProperty(root, "data", out data))
            {
                if (data.ValueKind == JsonValueKind.Array)
                {
                    return data.EnumerateArray();
                }

                if ((TryGetProperty(data, "Data", out var nested) || TryGetProperty(data, "data", out nested)) && nested.ValueKind == JsonValueKind.Array)
                {
                    return nested.EnumerateArray();
                }
            }

            return Array.Empty<JsonElement>();
        }

        private static int? ExtractTotalPages(JsonElement root)
        {
            if (TryReadInt(root, "totalPages", out var value) || TryReadInt(root, "TotalPages", out value))
            {
                return value;
            }

            if ((TryGetProperty(root, "Data", out var data) || TryGetProperty(root, "data", out data)) &&
                (TryReadInt(data, "totalPages", out value) || TryReadInt(data, "TotalPages", out value)))
            {
                return value;
            }

            return null;
        }

        private static bool TryGetDate(JsonElement item, out DateTime createdAt)
        {
            createdAt = default;
            if (!TryGetProperty(item, "createdAt", out var value) && !TryGetProperty(item, "CreatedAt", out value))
            {
                return false;
            }

            return value.ValueKind == JsonValueKind.String && DateTime.TryParse(value.GetString(), out createdAt);
        }

        private static bool TryReadInt(JsonElement element, string name, out int value)
        {
            value = 0;
            if (!TryGetProperty(element, name, out var prop))
            {
                return false;
            }

            if (prop.ValueKind == JsonValueKind.Number)
            {
                return prop.TryGetInt32(out value);
            }

            return prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), out value);
        }

        private static bool TryGetProperty(JsonElement element, string propertyName, out JsonElement value)
        {
            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propertyName, out value))
            {
                return true;
            }

            value = default;
            return false;
        }

        private static RecordStats CalculateStats(List<DateTime> items)
        {
            var now = DateTime.UtcNow;
            var startToday = now.Date;
            var weekCutoff = now.AddDays(-7);
            var monthCutoff = now.AddDays(-30);

            return new RecordStats
            {
                Today = items.Count(d => d.ToUniversalTime() >= startToday),
                LastWeek = items.Count(d => d.ToUniversalTime() >= weekCutoff),
                LastMonth = items.Count(d => d.ToUniversalTime() >= monthCutoff),
                Total = items.Count
            };
        }

        private static void ApplyStats(RecordStats stats, Label today, Label lastWeek, Label lastMonth, Label total)
        {
            today.Text = stats.Today.ToString();
            lastWeek.Text = stats.LastWeek.ToString();
            lastMonth.Text = stats.LastMonth.ToString();
            total.Text = stats.Total.ToString();
        }

        private void SetLoadingState()
        {
            _domainsTodayLabel.Text = "...";
            _domainsLastWeekLabel.Text = "...";
            _domainsLastMonthLabel.Text = "...";
            _domainsTotalLabel.Text = "...";

            _customersTodayLabel.Text = "...";
            _customersLastWeekLabel.Text = "...";
            _customersLastMonthLabel.Text = "...";
            _customersTotalLabel.Text = "...";
        }

        private void SetUnavailableState()
        {
            SetUnavailable(_domainsTodayLabel, _domainsLastWeekLabel, _domainsLastMonthLabel, _domainsTotalLabel);
            SetUnavailable(_customersTodayLabel, _customersLastWeekLabel, _customersLastMonthLabel, _customersTotalLabel);
        }

        private static void SetUnavailable(Label today, Label lastWeek, Label lastMonth, Label total)
        {
            today.Text = "-";
            lastWeek.Text = "-";
            lastMonth.Text = "-";
            total.Text = "-";
        }

        private static Label CreateValueLabel()
        {
            return new Label
            {
                Text = "...",
                TextColor = (Color)Application.Current!.Resources["White"]
            };
        }

        private static View CreateStatsSection(string title, Label today, Label lastWeek, Label lastMonth, Label total)
        {
            var titleLabel = new Label
            {
                Text = title,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = (Color)Application.Current!.Resources["White"]
            };

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                RowSpacing = 8
            };

            AddStatRow(grid, 0, "Today", today);
            AddStatRow(grid, 1, "Last week", lastWeek);
            AddStatRow(grid, 2, "Last month", lastMonth);
            AddStatRow(grid, 3, "Total", total);

            return new Frame
            {
                BackgroundColor = (Color)Application.Current!.Resources["OffBlack"],
                BorderColor = (Color)Application.Current.Resources["Gray600"],
                CornerRadius = 12,
                Padding = new Thickness(14),
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        titleLabel,
                        grid
                    }
                }
            };
        }

        private static void AddStatRow(Grid grid, int rowIndex, string caption, Label valueLabel)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            var captionLabel = new Label
            {
                Text = caption,
                TextColor = (Color)Application.Current!.Resources["Gray200"]
            };

            Grid.SetRow(captionLabel, rowIndex);
            Grid.SetColumn(captionLabel, 0);

            Grid.SetRow(valueLabel, rowIndex);
            Grid.SetColumn(valueLabel, 1);

            grid.Children.Add(captionLabel);
            grid.Children.Add(valueLabel);
        }

        private sealed class RecordStats
        {
            public int Today { get; init; }
            public int LastWeek { get; init; }
            public int LastMonth { get; init; }
            public int Total { get; init; }
        }

        private sealed class LoadResult
        {
            public bool IsSuccess { get; init; }
            public List<DateTime> Items { get; init; } = new();
            public string ErrorMessage { get; init; } = string.Empty;

            public static LoadResult Success(List<DateTime> items) => new() { IsSuccess = true, Items = items };

            public static LoadResult Fail(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }
}
