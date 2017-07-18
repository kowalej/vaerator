using System;
using System.Collections.Generic;
using System.Text;

namespace Vaerator.Services
{
    public enum DeviceRotation { ROTATION0 = 0, ROTATION90 = 1, ROTATION180 = 2, ROTATION270 = 3}

    public interface IRotationService
    {
        DeviceRotation GetRotation();
    }
}
