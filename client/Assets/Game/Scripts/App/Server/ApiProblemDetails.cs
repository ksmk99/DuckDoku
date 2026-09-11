using System;

namespace DuckDoku.App
{
    [Serializable]
    public class ApiProblemDetails
    {
        public string title;
        public int status;
        public string code;
    }
}