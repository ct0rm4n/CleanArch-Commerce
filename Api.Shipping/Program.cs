using RestSharp;
using Newtonsoft.Json;
using Core.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Data.Interfaces;
using Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Core.Entities.Domain.User;
using Core.ViewModel.Address;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.MapPost("/api/shipping/add/address", async Task<Results<Ok, NotFound, ProblemHttpResult, UnauthorizedHttpResult>> (HttpRequest request,
    ICheckoutService setService, IUserService userService,
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
            }
            //var item = new ShoppingCartItem() { Quantity = qtd, ProductId = idProduto, UserId = user.Id, TotalPrice = 100 };
            //item = setService.Add(item);
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


app.Run();

