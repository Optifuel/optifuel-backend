namespace ApiCos.Response
{
    public class ResponseHandler
    {
        public static ApiResponse GetExceptionResponse(Exception ex)
        {
            ApiResponse response = new ApiResponse();
            response.Code = 1;
            response.Data = ex.Message;
            return response;
        }

        public static ApiResponse GetApiResponse(int id, string description ,object? contract = null)
        {
            ApiResponse response;

            response = new ApiResponse { Data = contract };
            response.Code = id;
            response.Message = description;

            return response;
        }

        public static ApiResponse GetApiResponse(ResponseType type, object? contract)
        {
            ApiResponse response;
            response = new ApiResponse{  Data = contract };
            switch(type)
            {
                case ResponseType.Success:
                    response.Code = 0;
                    response.Message = type.ToString();
                break;

                case ResponseType.Failure:
                    response.Code = 1;
                    response.Message = type.ToString();
                break;

                case ResponseType.NotFound:
                    response.Code = 2;
                    response.Message = type.ToString();
                break;
            }
            return response;
        }
    }
}
