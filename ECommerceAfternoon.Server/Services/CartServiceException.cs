namespace ECommerceAfternoon.Server.Services
{
    public class CartServiceException : Exception
    {
        public int StatusCode { get; }

        public CartServiceException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
