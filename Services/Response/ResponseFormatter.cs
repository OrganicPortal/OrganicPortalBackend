using System.Net;

namespace OrganicPortalBackend.Services.Response
{
    public class ResponseFormatter : ResponseBuilder<object>
    {
        public ResponseFormatter() { }
        public ResponseFormatter(HttpStatusCode type = HttpStatusCode.BadRequest, string message = "", object data = default(object)) : base(type, message, data) { }
    }
    public class ResponseFormatter<T> : ResponseBuilder<T>
    {
        public ResponseFormatter() { }
        public ResponseFormatter(HttpStatusCode type = HttpStatusCode.BadRequest, string message = "", T data = default(T)) : base(type, message, data) { }
    }
}
