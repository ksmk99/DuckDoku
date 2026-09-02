using DuckDoku.Contracts;

namespace DuckDoku.Api
{
    public class ApiException : Exception
    {
        public ApiException(ErrorCode code, string message) : base(message)
        {
            Code = code;
        }

        public ErrorCode Code { get; }
    }
}
