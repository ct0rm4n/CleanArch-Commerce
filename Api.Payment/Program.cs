using Data.Interfaces;
using Data.Ioc;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using Core.Dto.OpenPix;
using RestSharp;
using Service.Interfaces;
using Service.Filter;
using Azure.Core;
using Core.Entities.Domain.User;
using Service;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Description = "api key.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "basic"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payments Web Api", Version = "v1" });
});


builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureServices(x => x.AddAutofac()).UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacDataModule());
});
builder.Host.ConfigureServices(x => x.AddAutofac()).UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacPersistanceModule());
});

var app = builder.Build();
var auth = app.Configuration;


app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments Web Api"));


app.UseHttpsRedirection();


app.MapGet("/api/Payments", async Task<Results<Ok<ChargeRoot?>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> (HttpRequest request, ICheckoutService cartService, IUserService userService) =>
    {
        try
        {
            var token = string.Empty;
            var total = decimal.Zero;
            var user = new User();
            if (request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                token = authorizationHeader.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    user = await userService.GetCurrentUserByToken(token);
                    if (user is null)
                    {
                        return TypedResults.Unauthorized();
                    }
                }
                total = cartService.GetTotalByCustomerId(user.Id);
                var options = new RestClientOptions(auth["UrlPayment"])
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var requestApi = new RestRequest("/api/v1/charge", Method.Post);
                var apiKey = auth["ApiKeyPayment"];
                requestApi.AddHeader("Authorization", apiKey);
                requestApi.AddHeader("Content-Type", "application/json");
                var body = @"{
                " + "\n" +
                            @"    ""correlationID"": """ + Guid.NewGuid() + @""",
                " + "\n" +
                            @"    ""value"": " + total + @",
                " + "\n" +
                            @"    ""comment"" : ""loja""
                " + "\n" +
                            @"}";
                requestApi.AddStringBody(body, DataFormat.Json);
                RestResponse response = await client.ExecuteAsync(requestApi);
                var respones = JsonConvert.DeserializeObject<ChargeRoot>(response.Content);
                return TypedResults.Ok(respones);
            }

            return TypedResults.Unauthorized();
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }).AddEndpointFilter<AuthorizationLoggedActionFilter>();

app.Run();