using Nancy;

namespace BCR
{
    public class AuthModule : NancyModule
    {
        public AuthModule() 
          : base(Database.Instance.GlobalSettings.url_base + "/auth")
        {
            ///////////////////////////////////////////////////////////////////////////////////////////
            // Login
            // The Post["/"] method returns the api key for subsequent REST calls.
            Post["/"] = x =>
                {
                    string apiKey = UserDatabase.LoginUser((string) this.Request.Form.Username,
                                                           (string) this.Request.Form.Password);

                    return string.IsNullOrEmpty(apiKey)
                               ? new Response {StatusCode = HttpStatusCode.Unauthorized}
                               : this.Response.AsJson(new {ApiKey = apiKey});
                };

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Logout
            // Destroy the api key.
            Delete["/"] = x =>
                {
                    var apiKey = (string) this.Request.Form.ApiKey;
                    UserDatabase.RemoveApiKey(apiKey);
                    return new Response {StatusCode = HttpStatusCode.OK};
                };
        }
    }
}