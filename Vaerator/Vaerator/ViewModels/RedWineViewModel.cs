using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Vaerator.ViewModels
{
	public class RedWineViewModel : BaseViewModel
	{
		public RedWineViewModel()
		{
			Title = "About";
            //StartVibrate = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
            //CancelVibrate = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        /// <summary>
        /// Command to start the vibration (aeration) sequence.
        /// </summary>
        public ICommand StartVibrate { get; }

        /// <summary>
        /// Command to cancel the vibration (aeration) sequence.
        /// </summary>
        public ICommand CancelVibrate { get; }
	}
}
