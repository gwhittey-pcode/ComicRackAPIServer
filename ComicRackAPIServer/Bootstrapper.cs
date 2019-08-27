using BCR;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.TinyIoc;
using System;
using System.IO;

namespace ComicRackAPIServer
{
  /*
  public class LoggingErrorHandler : IErrorHandler
  {
      public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
      {
          return true;//statusCode == HttpStatusCode.InternalServerError;
      }
  
      public void Handle(HttpStatusCode statusCode, NancyContext context)
      {
        
          object errorObject;
          context.Items.TryGetValue(NancyEngine.ERROR_EXCEPTION, out errorObject);
          var error = errorObject as Exception;
          
  

      }
  }
  */

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private MyRootPathProvider myRootPathProvider;
        
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            
            // Increase size of JSON responses as 100K is way too small for a large comic collection. Make it 10M.
            // Also, for some reason I don't get InvalidOperationException ("Nancy.Json.JsonSettings.MaxJsonLength exceeded")
            // Instead Nancy generates a response with status OK and content length 0.
            Nancy.Json.JsonSettings.MaxJsonLength = Int32.MaxValue;
            Nancy.Json.JsonSettings.RetainCasing = true;

            // Case sensitivity is buggy in Nancy, so disable it. Or maybe I should generate/parse GUIDs correctly......
            StaticConfiguration.CaseSensitive = false;
            
            
            StaticConfiguration.EnableRequestTracing = Database.Instance.GlobalSettings.nancy_request_tracing;
            
            if (Database.Instance.GlobalSettings.nancy_diagnostics_password == "")
              DiagnosticsHook.Disable(pipelines);
            
            // Make sure static content isn't cached, because this really messes up the ipad browsers (Atomic Browser specifically) 
            // when the app is frequently updated.
          pipelines.AfterRequest.AddItemToEndOfPipeline(
            ctx => {
              ctx.Response.Headers["Cache-Control"] = "no-cache";
            });
        }
       
        protected override IRootPathProvider RootPathProvider
        {
          get { return this.myRootPathProvider ?? (this.myRootPathProvider = new MyRootPathProvider()); }
        }

        protected override void ConfigureConventions(NancyConventions conventions)
	      {
	        base.ConfigureConventions(conventions);
	 
	        conventions.StaticContentsConventions.Add(
              StaticContentConventionBuilder.AddDirectory(Database.Instance.GlobalSettings.url_base, "/tablet")
        	);
	      }
        
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
          get { return new DiagnosticsConfiguration { Password = Database.Instance.GlobalSettings.nancy_diagnostics_password }; }
        }
        

        
        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            // At request startup we modify the request pipelines to
            // include stateless authentication
            //
            // Configuring stateless authentication is simple. Just use the 
            // NancyContext to get the apiKey. Then, use the apiKey to get 
            // your user's identity.
            var configuration =
                new StatelessAuthenticationConfiguration(nancyContext =>
                    {
                        // For now, we will get the apiKey from a cookie.
                        // If there's no cookie, check the query string.
                        
                        try
                        {
                          string apiKey = "";
                          if (!nancyContext.Request.Cookies.TryGetValue("API_apiKey", out apiKey))
                          {
                            apiKey = (string) nancyContext.Request.Query.ApiKey.Value;
                          }
                          
                          return BCR.UserDatabase.GetUserFromApiKey(apiKey);
                        }
                        catch (Exception e)
                        {
                          System.Windows.Forms.MessageBox.Show("Error while initializing the user data:\n" + e.ToString(), "BCR Error");
                          Console.WriteLine(e.ToString());
                        }
                                                
                        return null;
                    });

            AllowAccessToConsumingSite(pipelines);

            StatelessAuthentication.Enable(pipelines, configuration);
        }

        static void AllowAccessToConsumingSite(IPipelines pipelines)
        {
            pipelines.AfterRequest.AddItemToEndOfPipeline(x =>
                {
                    x.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    x.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,DELETE,PUT,OPTIONS");
                    x.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                });
        }
    
    }

    public class MyRootPathProvider : IRootPathProvider
    {
        private readonly string BASE_PATH;

        public MyRootPathProvider()
        {
            var path = typeof(Bootstrapper).Assembly.Location;
            BASE_PATH = path.Substring(0, path.Length - Path.GetFileName(path).Length);
        }

        public string GetRootPath()
        {
            return BASE_PATH;
        }
    }

    
}
