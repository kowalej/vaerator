using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Vaerator.Services
{
    public class CrossRotationService
    {
        private static volatile IRotationService instance;
        private static object syncRoot = new Object();

        public static IRotationService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = DependencyService.Get<IRotationService>();
                    }
                }
                return instance;
            }
        }
    }
}
