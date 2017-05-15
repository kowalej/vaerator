using System;
using System.Collections.Generic;
using System.Text;

namespace Vaerator.Helpers
{
    class DeviceOrientationInfo
    {
        private static volatile DeviceOrientationInfo instance;
        private static object syncRoot = new Object();

        double width;
        double height;

        private DeviceOrientationInfo() { }
        public static DeviceOrientationInfo Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DeviceOrientationInfo();
                    }
                }
                return instance;
            }
        }

        public static bool IsOrientationPortrait()
        {
            return Instance.height > Instance.width;
        }

        public static void SetSize(double width, double height)
        {
            instance.width = width;
            instance.height = height;
        }
    }
}
