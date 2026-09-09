using System;

namespace DuckDoku.Domain
{
    public interface ICurrencyService
    {
        int Balance { get; }

        event Action Changed;
        event Action<int> Denied;

        void Apply(int balance);

        bool HasEnough(int cost);

        void NotifyDenied(int cost);
    }
}
