using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace Vaerator.Droid.Views
{
    public class BasePageRenderer : PageRenderer
    {/*
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var page = this.Element as BasePage;
            page.LeftToolbarItems.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => {
                UpdateView();
            };
        }

        private void UpdateView()
        {
            var leftToolbarItems = (this.Element as BasePage).LeftToolbarItems;
            if (leftToolbarItems.Count != 0)
            {
                NavigationController.TopViewController.NavigationItem.LeftBarButtonItems = GetBarButtonItems(leftToolbarItems);
            }
            else
            {
                NavigationController.TopViewController.NavigationItem.LeftBarButtonItems = new UIBarButtonItem[] { };
            }
        }

        private UIBarButtonItem[] GetBarButtonItems(IEnumerable<ToolbarItem> items)
        {
            var leftBarButtonItem = new UIBarButtonItem();
            foreach (var item in items)
            {
                if (item.Priority == 1)
                {
                    leftBarButtonItem = new UIBarButtonItem(UIImage.FromFile(item.Icon), UIBarButtonItemStyle.Plain, ((object sender, EventArgs e) => {
                        item.Command.Execute(null);
                    }));
                }
            }

            return new UIBarButtonItem[] { leftBarButtonItem };
        }*/
    }
}