using BCR;
using Nancy;
using Nancy.Responses;

namespace ComicRackAPIServer
{
  public class IndexModule : NancyModule
  {
    public IndexModule()
      : base("/")
    {
      Get["/"] = x => {
        var baseUrl = Database.Instance.GlobalSettings.url_base;
        if (string.IsNullOrEmpty(baseUrl))
        {
          baseUrl = "/index.html";
        }
        else
        {
          baseUrl = "/" + baseUrl + "/index.html";
        }

        return Response.AsRedirect(baseUrl, RedirectResponse.RedirectType.Permanent); 
      };
    }
  }
}
