using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public ApiResponse(bool success, int statusCode, string message, T? data) 
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }

        public static ApiResponse<T> SuccessResponse(int statusCode,string message, T? data)
        => new ApiResponse<T>(true, statusCode, message, data);

        public static ApiResponse<T> FailureResponse(int statusCode, string message)
        => new ApiResponse<T>(false, statusCode,  message, default);

    }
}
