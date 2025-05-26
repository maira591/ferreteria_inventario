using System;

namespace Core.Application.Website.Utils
{
    public static class JsonResponseFactory
    {
        public static object ErrorResponse(Exception error)
        {
            return new { Success = false, Message = error.Message };
        }

        public static object ErrorResponse(string error)
        {
            return new { Success = false, Message = error };
        }

        public static object SuccessResponse()
        {
            return new { Success = true, Message = string.Empty };
        }

        public static object SuccessResponse(string message)
        {
            return new { Success = true, Message = message };
        }

        public static object SuccessResponse(object referenceObject)
        {
            return new { Success = true, Message = string.Empty, Object = referenceObject };
        }

        public static object SuccessResponse(string message, object referenceObject)
        {
            return new { Success = true, Message = message, Object = referenceObject };
        }
    }
}