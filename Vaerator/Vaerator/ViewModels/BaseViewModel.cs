using Vaerator.Helpers;

namespace Vaerator.ViewModels
{
	public class BaseViewModel : ObservableObject
	{
        public Settings Settings
        {
            get { return Settings.Current; }
        }

		/// <summary>
		/// Private backing field to hold the title.
		/// </summary>
		string title = string.Empty;
		/// <summary>
		/// Get and set the title of the item.
		/// </summary>
		public string Title
		{
			get { return title; }
			set { SetProperty(ref title, value); }
		}
    }
}

