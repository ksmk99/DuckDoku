using System;

namespace DuckDoku.App
{
    public interface IIdempotentRequest
    {
        Guid RequestId { get; }
    }
}