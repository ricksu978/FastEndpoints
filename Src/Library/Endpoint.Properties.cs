﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FastEndpoints;

public abstract partial class Endpoint<TRequest, TResponse> : BaseEndpoint where TRequest : notnull, new() where TResponse : notnull, new()
{
    /// <summary>
    /// indicates if there are any validation failures for the current request
    /// </summary>
    public bool ValidationFailed => ValidationFailures.Count > 0;

    /// <summary>
    /// the current user principal
    /// </summary>
    protected ClaimsPrincipal User => HttpContext.User;

    /// <summary>
    /// the response that is sent to the client.
    /// </summary>
    protected TResponse Response { get; set; } = new();

    /// <summary>
    /// gives access to the configuration
    /// </summary>
    protected IConfiguration Config => HttpContext.RequestServices.GetRequiredService<IConfiguration>();

    /// <summary>
    /// gives access to the hosting environment
    /// </summary>
    protected IWebHostEnvironment Env => HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();

    /// <summary>
    /// the logger for the current endpoint type
    /// </summary>
    protected ILogger Logger => HttpContext.RequestServices.GetRequiredService<ILogger<Endpoint<TRequest, TResponse>>>();

    /// <summary>
    /// the base url of the current request
    /// </summary>
    protected string BaseURL => HttpContext.Request?.Scheme + "://" + HttpContext.Request?.Host + "/";

    /// <summary>
    /// the http method of the current request
    /// </summary>
    protected Http HttpMethod => Enum.Parse<Http>(HttpContext.Request.Method);

    /// <summary>
    /// the form sent with the request. only populated if content-type is 'application/x-www-form-urlencoded' or 'multipart/form-data'
    /// </summary>
    protected IFormCollection Form => HttpContext.Request.Form;

    /// <summary>
    /// the files sent with the request. only populated when content-type is 'multipart/form-data'
    /// </summary>
    protected IFormFileCollection Files => Form.Files;
}

