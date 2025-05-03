using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Net;

namespace OrganicPortalBackend.Services.Response
{
    public class ResponseBuilder<T>
    {
        public ResponseBuilder() { }
        public ResponseBuilder(HttpStatusCode type = HttpStatusCode.BadRequest, string message = "", T data = default(T))
        {
            Type = type;
            Message = message;
            Data = data;
        }

        public HttpStatusCode Type { get; set; } = HttpStatusCode.BadRequest;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; } = default(T);


        public IActionResult Result
        {
            get
            {
                dynamic response = new ExpandoObject();

                if (Data != null)
                    response.Data = Data;

                if (!string.IsNullOrWhiteSpace(Message))
                    response.Message = Message;

                return new DynamicObjectResult(Type, response);
            }
        }

        private class DynamicObjectResult : ObjectResult
        {
            public DynamicObjectResult(HttpStatusCode code, object value) : base(value)
            {
                base.StatusCode = (int)code;
            }
        }
    }
}
