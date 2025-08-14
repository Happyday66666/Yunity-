namespace Yunity.Models
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request) 
        { 
            if (request == null) 
            { 
                throw new ArgumentNullException("request"); 
            }
            
            return request.Headers["X-Requested-With"] == "XMLHttpRequest"; 
        }
    }
}
