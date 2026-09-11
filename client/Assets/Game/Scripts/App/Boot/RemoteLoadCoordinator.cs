using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DuckDoku.App
{
    public class RemoteLoadCoordinator
    {
        private readonly List<IRemoteLoadable> _loadables;

        public RemoteLoadCoordinator(List<IRemoteLoadable> loadables)
        {
            _loadables = loadables;
        }

        public UniTask LoadAllAsync(CancellationToken cancellationToken)
        {
            List<UniTask> tasks = new List<UniTask>(_loadables.Count);

            foreach (IRemoteLoadable loadable in _loadables)
            {
                tasks.Add(loadable.LoadAsync(cancellationToken));
            }

            return UniTask.WhenAll(tasks);
        }
    }
}
