using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface IRemoteLoadable
    {
        UniTask LoadAsync(CancellationToken cancellationToken);
    }
}
