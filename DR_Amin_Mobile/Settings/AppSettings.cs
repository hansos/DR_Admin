namespace DR_Amin_Mobile.Settings
{
    public static class AppSettings
    {
        public const string ApiBaseUrlPreferenceKey = "api_base_url";
        public const string DefaultApiBaseUrl = "https://127.0.0.1:7201";

        public static bool HasStoredApiBaseUrl()
        {
            var value = Preferences.Get(ApiBaseUrlPreferenceKey, string.Empty);
            return !string.IsNullOrWhiteSpace(value);
        }

        public static string GetApiBaseUrl()
        {
            var value = Preferences.Get(ApiBaseUrlPreferenceKey, DefaultApiBaseUrl).Trim();
            return value.TrimEnd('/');
        }

        public static void SetApiBaseUrl(string value)
        {
            Preferences.Set(ApiBaseUrlPreferenceKey, value.Trim().TrimEnd('/'));
        }
    }
}
