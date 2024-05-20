using RestSharp;
using Newtonsoft.Json;
using Core.Dto;

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


app.MapGet("/shipping/getAddress", (string cep) =>
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
})
.WithName("getAddress")
.WithOpenApi();

app.Run();

