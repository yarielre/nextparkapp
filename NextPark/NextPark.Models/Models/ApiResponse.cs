using NextPark.Enums.Enums;

namespace NextPark.Models
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
        
        public ErrorType ErrorType { get; set; }

        public object Result { get; set; }

        public static ApiResponse GetErrorResponse(string message, ErrorType errorType) {
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


}

