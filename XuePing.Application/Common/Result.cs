using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Application.Common
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string? ErrorMsg { get; set; }
        public T? Data { get; set; }

        public static Result<T> Ok(T data) => new() { Success = true , Data = data };
        public static Result<T> Fail(string errorMsg) => new() { Success = false, ErrorMsg = errorMsg };
    }
}
