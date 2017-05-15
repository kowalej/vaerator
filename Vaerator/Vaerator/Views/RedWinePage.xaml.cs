using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using System.Threading.Tasks;
using Vaerator.ViewModels;
using FFImageLoading.Forms;
using System;

namespace Vaerator.Views
{
    public partial class RedWinePage : BeverageBasePage
    {
        RedWineViewModel vm;
        public RedWinePage()
        {
            InitializeComponent();

            //Set custom color for red wine.
            fluidColor = new Color(0.55f, 0, 0);
            SetupFluidSim(WineContainer, "red_wine_staticbg.jpg");
            vm = (RedWineViewModel)BindingContext;
            vm.InitializeDefaults();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DurationSlider.StyleId = "durationSlider";
        }

        protected override void OnDisappearing()
        {
            vm.SavePrefs();
            base.OnDisappearing();
        }
    }
}
