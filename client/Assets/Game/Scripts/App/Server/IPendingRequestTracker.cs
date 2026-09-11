using System;
using System.Collections.Generic;

namespace DuckDoku.App
{
    public interface IPendingRequestTracker
    {
        Guid GetOrCreateGuid(string actionKey);
        void Resolve(string actionKey);
    }

    public class PendingRequestTracker : IPendingRequestTracker
    {
        private Dictionary<string, Guid> _actions = new Dictionary<string, Guid>();
        
        public Guid GetOrCreateGuid(string actionKey)
        {
            if (_actions.TryGetValue(actionKey, out var guid))
            {
                return guid;
            }
            else
            {
                _actions[actionKey] = Guid.NewGuid();
                return _actions[actionKey];
            }
        }

        public void Resolve(string actionKey)
        {
            _actions.Remove(actionKey);
        }
    }
}