using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Vaerator.ViewModels
{
	public class RedWineViewModel : BaseViewModel
	{
		public RedWineViewModel()
		{
			
        }

        /// <summary>
        /// Sets the duration of the red wine aeration.
        /// </summary>
        public ICommand Duration { get; }
	}
}
