using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Vaerator.Services
{
    public class CrossKeepAwakeService
    {
        private static volatile IKeepAwakeService instance;
        private static object syncRoot = new Object();

        public static IKeepAwakeService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = DependencyService.Get<IKeepAwakeService>();
                    }
                }
                return instance;
            }
        }
    }
}
