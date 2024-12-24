using Data.Interfaces;
using Data.Ioc;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Service.Filter;
using Service.Interfaces;
using Core.Entities.Domain.Checkout;
using Core.Entities.Domain.User;


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

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Checkout Web Api", Version = "v1" });
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

//builder.Services.InjectConfigureServices();
builder.Services.AddSingleton<AuthorizationLoggedActionFilter>();

var app = builder.Build();
var auth = app.Configuration;

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Checkout Web Api"));

app.UseHttpsRedirection();



app.MapPost("/api/Checkout/add/{idProduto:int}", async (HttpRequest request,
    ICheckoutService setService, IUserService userService,
    int idProduto, [FromQuery] int qtd) =>
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
            }
        }        

        var item = new ShoppingCartItem(){ Quantity = qtd, ProductId = idProduto, UserId = user.Id, TotalPrice = 100 };
        item = setService.Add(item);
        return TypedResults.Ok();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}).AddEndpointFilter<AuthorizationLoggedActionFilter>();


app.Run();


