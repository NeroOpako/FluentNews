using FluentNews.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace FluentNews.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (ViewModel.Settings.Values.ContainsKey("username"))
        {
            usernameTextBox.Text = ViewModel.Settings.Values["username"] as String;
        }
        if (ViewModel.Settings.Values.ContainsKey("host"))
        {
            urlTextBox.Text = ViewModel.Settings.Values["host"] as String;
        }
        if (ViewModel.Settings.Values.ContainsKey("password"))
        {
            passwordBox.Password = ViewModel.Settings.Values["password"] as String;
        }

        if (ViewModel.Settings.Values.ContainsKey("refresh"))
        {
            switch ((int)ViewModel.Settings.Values["refresh"])
            {
                case 1:
                    comboBox.SelectedItem = OneMinuteItem;
                    break;
                case 5:
                    comboBox.SelectedItem = FiveMinuteItem;
                    break;
                case 10:
                    comboBox.SelectedItem = TenMinuteItem;
                    break;
                case 15:
                    comboBox.SelectedItem = FifteenMinuteItem;
                    break;
                case 30:
                    comboBox.SelectedItem = ThirtyMinuteItem;
                    break;
                case 60:
                    comboBox.SelectedItem = OneHourItem;
                    break;
                case 480:
                    comboBox.SelectedItem = EightHourItem;
                    break;
                case 1440:
                    comboBox.SelectedItem = TwentyFourHourItem;
                    break;
                default:
                    comboBox.SelectedItem = NoRefreshItem;
                    break;
            }
        }
        else
        {
            comboBox.SelectedItem = NoRefreshItem;
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Settings.Values["host"] = urlTextBox.Text;
        ViewModel.Settings.Values["username"] = usernameTextBox.Text;
        ViewModel.Settings.Values["password"] = passwordBox.Password;

        var item = comboBox.SelectedItem as ComboBoxItem;

        switch (item.Name)
        {
            case "OneMinuteItem":
                ViewModel.Settings.Values["refresh"] = 1;
                break;
            case "FiveMinuteItem":
                ViewModel.Settings.Values["refresh"] = 5;
                break;
            case "TenMinuteItem":
                ViewModel.Settings.Values["refresh"] = 10;
                break;
            case "FifteenMinuteItem":
                ViewModel.Settings.Values["refresh"] = 15;
                break;
            case "ThirtyMinuteItem":
                ViewModel.Settings.Values["refresh"] = 30;
                break;
            case "OneHourItem":
                ViewModel.Settings.Values["refresh"] = 60;
                break;
            case "EightHourItem":
                ViewModel.Settings.Values["refresh"] = 480;
                break;
            case "TwnetyFourHourItem":
                ViewModel.Settings.Values["refresh"] = 1440;
                break;
            default:
                ViewModel.Settings.Values["refresh"] = 0;
                break;
        }

    }
}
