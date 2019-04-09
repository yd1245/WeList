

using System;
using System.Linq;
using System.Web.Mvc;

namespace WeList.Helper
{
    public static class MyHelper
    {
        public static TResult Isolate<TResult>(this UrlHelper urlHelper, Func<UrlHelper, TResult> action)
        {
            var currentData = urlHelper.RequestContext.RouteData.Values.ToDictionary(kvp => kvp.Key);
            urlHelper.RequestContext.RouteData.Values.Clear();
            try
            {
                return action(urlHelper);
            }
            finally
            {
                foreach (var kvp in currentData)
                    urlHelper.RequestContext.RouteData.Values.Add(kvp.Key, kvp.Value.Value);
            }
        }
    }




}