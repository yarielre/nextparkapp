using NextPark.Enums.Enums;

namespace NextPark.Models
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public ErrorType ErrorType { get; set; }

        public object Result { get; set; }

        public static ApiResponse GetErrorResponse(string message, ErrorType errorType)
        {
            return new ApiResponse
            {
                ErrorType = errorType,
                IsSuccess = false,
                Message = message,
                Result = null
            };
        }

        public static ApiResponse GetSuccessResponse(object result, string message = "")
        {
            return new ApiResponse
            {
                ErrorType = ErrorType.None,
                IsSuccess = true,
                Message = message,
                Result = result
            };
        }
    }


    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public ErrorType ErrorType { get; set; }

        public T Result { get; set; }

        public static ApiResponse<T> GetErrorResponse(string message, ErrorType errorType)
        {
            return new ApiResponse<T>
            {
                ErrorType = errorType,
                IsSuccess = false,
                Message = message,
                Result = default(T)
            };
        }

        public static ApiResponse<T> GetSuccessResponse(T result, string message = "")
        {
            return new ApiResponse<T>
            {
                ErrorType = ErrorType.None,
                IsSuccess = true,
                Message = message,
                Result = result
            };
        }
    }


}

