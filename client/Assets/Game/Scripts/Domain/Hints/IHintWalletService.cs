using System;

namespace DuckDoku.Domain
{
    public interface IHintWalletService
    {
        int Count { get; }

        event Action Changed;
        event Action<int> Denied;

        void Apply(int count);

        bool HasEnough(int cost);

        void NotifyDenied(int cost);
    }
}
