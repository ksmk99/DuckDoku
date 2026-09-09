using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface IHintWalletClient
    {
        UniTask<HintsStateResponse> GetState(CancellationToken cancellationToken = default);

        UniTask<PurchaseHintResponse> Purchase(CancellationToken cancellationToken = default);
    }
}
