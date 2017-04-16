﻿using Vaerator.Helpers;
using Vaerator.Models;
using Vaerator.Services;

using Xamarin.Forms;

namespace Vaerator.ViewModels
{
	public class BaseViewModel : ObservableObject
	{
		/// <summary>
		/// Get the azure service instance.
		/// </summary>
		public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        /// <summary>
        /// Private backing field to hold busy state.
        /// </summary>
		bool isBusy = false;
        /// <summary>
        /// Get and set whether the data service is busy. 
        /// </summary>
		public bool IsBusy
		{
			get { return isBusy; }
			set { SetProperty(ref isBusy, value); }
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

