using Microsoft.Practices.Unity;
using System.Web.Http;
using RestSharp;
using twitter_api.Services;
using Unity.WebApi;

namespace twitter_api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IRestClient, RestClient>(new InjectionConstructor());
            container.RegisterType<ITwitterService, TwitterService>(
                new InjectionConstructor(typeof(IRestClient), typeof(string), typeof(string), typeof(string), typeof(string)));

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}