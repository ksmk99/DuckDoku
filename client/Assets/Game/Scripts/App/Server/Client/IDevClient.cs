#if UNITY_EDITOR
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface IDevClient
    {
        UniTask<DevEnergyResponse> GrantMaxEnergy(CancellationToken cancellationToken = default);
    }
}
#endif
