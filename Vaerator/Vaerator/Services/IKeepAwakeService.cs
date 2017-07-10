using System;
using System.Collections.Generic;
using System.Text;

namespace Vaerator.Services
{
    public interface IKeepAwakeService
    {
        void StartAwake();
        void StopAwake();
    }
}
