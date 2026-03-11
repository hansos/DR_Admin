using DR_Amin_Mobile.Settings;

namespace DR_Amin_Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            if (!AppSettings.HasStoredApiBaseUrl())
            {
                Dispatcher.Dispatch(async () => await GoToAsync("//ApiSettingsPage"));
            }
        }
    }
}
