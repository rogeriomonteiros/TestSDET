using Selenium.utils.Domain;

namespace Selenium.utils.Utils
{
    public class APIUtils
    {
        public APIUtils()
        {
            this._response = new Response();
        }

        private Response _response;
        public  Response APIJson(string endPoint, string imputJson)
        {
            //Abstraction of API caller
            return this._response;
        }

        public void SetResponse(string statusCode, string message)
        {
            this._response.StatusCode = statusCode;
            this._response.Message = message;

        }

        public static LoginBody GetLoginBody(string email, string password)
        {

            return new LoginBody() { Email = email, Password = password };
        }

        public static AccessDocumentBody GetAccessDocumentBody(string userId, string DocId)
        {

            return new AccessDocumentBody() { UserId = userId, DocId = DocId };
        }


    }
}
