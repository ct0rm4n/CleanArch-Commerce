using RestSharp;
using Newtonsoft.Json;
using Core.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Data.Interfaces;
using Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Core.Entities.Domain.User;
using Core.ViewModel.Address;
using Service.Helpers;
using Core.Entities.Domain.Address;
using Autofac;
using Data.Ioc;
using Microsoft.OpenApi.Models;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Service.Ioc;

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

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shipping Web Api", Version = "v1" });
});

builder.Services.InjectConfigureServices();

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
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shipping Web Api"));

var mapper = app.Services.GetService<IMapper>();

app.UseHttpsRedirection();

app.MapGet("api/shipping/getAddress", (string cep) =>
{
    try
    {
        var options = new RestClientOptions("https://brasilapi.com.br")
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var request = new RestRequest($"/api/cep/v1/{cep}", Method.Get);
        RestResponse response = client.Execute(request);
        return Results.Ok(JsonConvert.DeserializeObject<CepDto>(response.Content));
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.MapGet("api/shipping/getAddress/viacep", async Task<Results<Ok<ResponseAddressVM>, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> (IAddressService addressService, [FromQuery]string cep) =>
{
    try
    {
        var options = new RestClientOptions($"https://viacep.com.br")
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var requestApi = new RestRequest($"/ws/{cep}/json", Method.Get);
        RestResponse response = client.Execute(requestApi);
        var reponseDto = JsonConvert.DeserializeObject<ViacepDto>(response.Content);
        var stateEnity = addressService.GetStatesByUf(reponseDto.uf);
        var states = mapper.Map<StatesVM>(stateEnity);
        var cityEnity = addressService.GetCityByName(reponseDto.localidade);
        var city = mapper.Map<CityVM>(cityEnity);
        var responseVM = new ResponseAddressVM(reponseDto, states, city);
        return TypedResults.Ok(responseVM);
    }
    catch (Exception ex)
    {
        return TypedResults.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});


app.MapPost("/api/shipping/add/address", async Task<Results<Ok, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> (HttpRequest request,
    ICheckoutService setService, IUserService userService, IAddressService addressService,
    [FromBody] AddressVM address) =>
{
    try
    {
        var token = string.Empty;
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
                address.UserId = user.Id;
                var serialized = JsonConvert.SerializeObject(address, new JsonSerializerSettings()
                {
                    ContractResolver = new IgnorePropertiesResolver(new[] { "Id" })
                });
                List<string> validation = addressService.GetValidation(address).ToList();
                if (validation is not null && validation.Count() > 0)
                    return TypedResults.Problem(JsonConvert.SerializeObject(validation));

                var insert = addressService.Add(JsonConvert.DeserializeObject<Address>(serialized));
                return insert is not null
                ? TypedResults.Ok()
                : TypedResults.NotFound();
            }

            return TypedResults.Ok();
        }
        else
        {
            return TypedResults.Unauthorized();
        }


    }
    catch (Exception ex)
    {
        return TypedResults.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});//.AddEndpointFilter<AuthorizationLoggedActionFilter>();

app.MapGet("/api/shipping/getStates", async (IAddressService addressService) =>
{
    try
    {
        var insert = addressService.GetStates();
        return insert is not null
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.MapGet("/api/shipping/CityByState", async (IAddressService addressService, [FromQuery] int stateId) =>
{
    try
    {
        var insert = addressService.GetCityByStates(stateId);
        return insert is not null
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.Run();

