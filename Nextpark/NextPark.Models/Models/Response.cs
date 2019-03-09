using NextPark.Enums.Enums;

namespace NextPark.Models
{
    public class Response
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
        
        public ErrorType ErrorType { get; set; }

        public object Result { get; set; }

        public static Response GetErrorResponse(string message, ErrorType errorType) {
            return new Response
            {
                ErrorType = errorType,
                IsSuccess = false,
                Message = message,
                Result = null
            };
        }

        public static Response GetSuccessResponse(object result, string message = "")
        {
            return new Response
            {
                ErrorType = ErrorType.None,
                IsSuccess = true,
                Message = message,
                Result = result
            };
        }
    }


}

