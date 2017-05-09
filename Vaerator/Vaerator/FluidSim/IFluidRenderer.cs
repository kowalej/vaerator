using System.Threading.Tasks;
using Xamarin.Forms;

namespace Vaerator.FluidSim
{
    public interface IFluidRenderer
    {
        void Setup(float resolution, out int N, out int M);
        Task Render(int timeRemaining);
        void SetColor(Color baseColor, float colorScaleFactor = 1.0f);
        void SetDensity(ref float[] d);
        void SetVelocity(ref float[] u, ref float[] v);
    }
}
