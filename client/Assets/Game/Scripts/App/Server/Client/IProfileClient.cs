using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public interface IProfileClient
    {
        UniTask<PlayerProfile> GetProfile(CancellationToken cancellationToken = default);

        UniTask<PlayerProfile> ChangeName(string name, CancellationToken cancellationToken = default);
    }
}
