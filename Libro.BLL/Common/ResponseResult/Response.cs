using System.Net;
namespace Libro.BLL.Common.ResponseResult
{
    public record Response<T>(T? Result, string? ErrorMessage, bool HasErrorMessage, HttpStatusCode StatusCode = HttpStatusCode.OK);
    //public class Result<T>
    //{
    //public bool Success1 { get; }
    //public T? Data { get; }
    //public string? ErrorMessage { get; }
    //public HttpStatusCode StatusCode { get; }

    //private Result(bool success, T? data, string? errorMessage, HttpStatusCode statusCode)
    //{
    //    Success1 = success;
    //    Data = data;
    //    ErrorMessage = errorMessage;
    //    StatusCode = statusCode;
    //}

    //public static Result<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    //    => new(true, data, null, statusCode);

    //public static Result<T> Failure(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    //    => new(false, default, errorMessage, statusCode);
    //}
}
