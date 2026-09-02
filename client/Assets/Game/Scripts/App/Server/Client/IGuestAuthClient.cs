using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface IGuestAuthClient
    {
        UniTask AuthenticateAsGuestAsync(CancellationToken cancellationToken = default);
    }
}
