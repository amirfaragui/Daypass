using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ValueCards.Services
{
  /// <summary>
  /// Implementation of <see cref="IAuthenticationHandlerProvider"/>.
  /// </summary>
  public class MyAuthenticationHandlerProvider : IAuthenticationHandlerProvider
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="schemes">The <see cref="IAuthenticationHandlerProvider"/>.</param>
    public MyAuthenticationHandlerProvider(IAuthenticationSchemeProvider schemes)
    {
      Schemes = schemes;
    }

    /// <summary>
    /// The <see cref="IAuthenticationHandlerProvider"/>.
    /// </summary>
    public IAuthenticationSchemeProvider Schemes { get; }

    // handler instance cache, need to initialize once per request
    private Dictionary<string, IAuthenticationHandler> _handlerMap = new Dictionary<string, IAuthenticationHandler>(StringComparer.Ordinal);

    /// <summary>
    /// Returns the handler instance that will be used.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="authenticationScheme">The name of the authentication scheme being handled.</param>
    /// <returns>The handler instance.</returns>
    public async Task<IAuthenticationHandler> GetHandlerAsync(HttpContext context, string authenticationScheme)
    {
      var routeData = context.GetRouteData();
      var area = routeData?.Values["area"]?.ToString().ToUpper();

      area = area ?? authenticationScheme;

      if (_handlerMap.ContainsKey(area))
      {
        return _handlerMap[area];
      }

      var scheme = await Schemes.GetSchemeAsync(area);
      if (scheme == null)
      {
        //return null;


        //var endpointFeature = context.Features[typeof(IEndpointFeature)] as IEndpointFeature;
        //var endpoint = endpointFeature?.Endpoint;

        //var routePattern = (endpoint as RouteEndpoint)?.RoutePattern;
        //var area = routePattern?.PathSegments[0].ToString();

        //if (!string.IsNullOrEmpty(area))
        //{
        //  scheme = await Schemes.GetSchemeAsync(area.ToUpper());
        //}

        if (scheme == null)
        {
          scheme = await Schemes.GetDefaultAuthenticateSchemeAsync();
        }
      }

      var handler = (context.RequestServices.GetService(scheme.HandlerType) ??
          ActivatorUtilities.CreateInstance(context.RequestServices, scheme.HandlerType))
          as IAuthenticationHandler;
      if (handler != null)
      {
        await handler.InitializeAsync(scheme, context);
        _handlerMap[authenticationScheme] = handler;
      }
      return handler;
    }
  }
}
