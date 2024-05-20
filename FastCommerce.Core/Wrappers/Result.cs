
namespace Core.Wrappers
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public List<string>? Errors { get; set; }
        public string Message { get; set; }

        public static Result<T> Success(T data)
        {
            return new Result<T> { Data = data };
        }
        public static Result<T> Success(T data, string message)
        {
            return new Result<T> { Data = data, Message = message };
        }
        public static Result<T> Success(int statusCode, T data)
        {
            return new Result<T> { Data = data, StatusCode = statusCode };
        }
        public static Result<T> Success(int statusCode)
        {
            return new Result<T> { StatusCode = statusCode };
        }

        public static Result<T> Fail(int statusCode, List<string> errors)
        {
            return new Result<T> { StatusCode = statusCode, Errors = errors };
        }

        public static Result<T> Fail(int statusCode, string error)
        {
            return new Result<T> { StatusCode = statusCode, Errors = new List<string> { error } };
        }

        public static Result<T> Fail(string message)
        {
            return new Result<T> { Message = message };
        }

    }
}