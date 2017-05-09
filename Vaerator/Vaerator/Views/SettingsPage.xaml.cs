using System;
using Vaerator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vaerator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : BasePage
    {
        public SettingsPage()
        {
			InitializeComponent();
        }

        private void LanguagePickerCell_OnTapped(object sender, EventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                LanguagePicker.Focus();
            });
        }

        private void BackgroundSimEnabledCell_OnTapped(object sender, EventArgs e)
        {
            BackgroundSimEnabledSwitch.IsToggled = BackgroundSimEnabledSwitch.IsToggled ? false : true;
        }
    }
}
