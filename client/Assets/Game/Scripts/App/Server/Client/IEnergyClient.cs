using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface IEnergyClient
    {
        UniTask<EnergyStateResponse> GetState(CancellationToken cancellationToken = default);
    }
}
