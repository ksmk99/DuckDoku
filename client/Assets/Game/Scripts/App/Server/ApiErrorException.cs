using System;

namespace DuckDoku.App
{
    public sealed class ApiErrorException : Exception
    {
        public int StatusCode { get; }
        public string Code { get; }

        public ApiErrorException(int statusCode, string code, string message) : base(message)
        {
            StatusCode = statusCode;
            Code = code;
        }
    }
}