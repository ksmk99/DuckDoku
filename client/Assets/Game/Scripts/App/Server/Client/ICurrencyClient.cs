using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface ICurrencyClient
    {
        UniTask<CurrencyStateResponse> GetBalance(CancellationToken cancellationToken = default);
    }
}
