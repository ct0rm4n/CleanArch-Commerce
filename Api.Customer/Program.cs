using Core.Wrappers;
using Data.Interfaces;
using Data.Ioc;
using Microsoft.AspNetCore.Http.HttpResults;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Core.Entities.Domain.User;
using Core.ViewModel.User;
using Microsoft.OpenApi.Models;
using Service.Filter;

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

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer Web Api", Version = "v1" });
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

//builder.Services.AddSingleton<Service.Middleware.AuthorizationMiddleware>();

var app = builder.Build();
var auth = app.Configuration;


app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Web Api"));


app.UseHttpsRedirection();

/*role*/
app.MapGet("/api/Role/Get/{id}", Results<Ok<Role>, NotFound> (IRoleService setService, int id, HttpContext context) =>
        setService.Get(id) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound()).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("/api/Role/GetALl",
    async (IRoleService setService, [FromQuery] int? PageNumber, [FromQuery] int? PageSize) =>
    {
        try
        {
            var paginationFilter = new PaginationFilter();
            if (PageNumber.HasValue && PageNumber > 0 && PageSize.HasValue && PageSize > 0)
                paginationFilter = new PaginationFilter((int)PageNumber, (int)PageSize);

            if ((PageNumber.HasValue && PageNumber > 0) && (PageSize == null || PageSize == 0))
                paginationFilter = new PaginationFilter((int)PageNumber, 10);

            var result = setService.Search(paginationFilter);

            if (result.Data is null || result.Data.Count == 0)
                return Results.NotFound("No Role posts found matching the filter.");

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("/api/Role/Search",
    async ([FromQuery] string filterText, [FromQuery] int? PageNumber, [FromQuery] int? PageSize, [FromServices] IRoleService setService) =>
    {
        try
        {
            var paginationFilter = new PaginationFilter();
            if (PageNumber.HasValue && PageNumber > 0 && PageSize.HasValue && PageSize > 0)
                paginationFilter = new PaginationFilter((int)PageNumber, (int)PageSize, filterText);

            if ((PageNumber.HasValue && PageNumber > 0) && (PageSize == null || PageSize == 0))
                paginationFilter = new PaginationFilter((int)PageNumber, 10, filterText);

            if (!string.IsNullOrEmpty(filterText))
                paginationFilter.SerachText = filterText;

            var result = setService.Search(paginationFilter);
            if (result.Data is null || result.Data.Count == 0)
                return Results.NotFound("No blog posts found matching the filter.");

            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }).AddEndpointFilter<AuthorizationActionFilter>();

app.MapPost("/api/Role/Add", async (IRoleService setService, [FromBody] RoleVM body) =>
{
    try
    {
        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = setService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = setService.Add(JsonConvert.DeserializeObject<Role>(serialized));
        return insert is not null
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}).AddEndpointFilter<AuthorizationActionFilter>();


app.MapPost("/api/Role/update", async (IRoleService setService, [FromBody] RoleVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = setService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = setService.Update(JsonConvert.DeserializeObject<Role>(serialized));
        return insert is not false
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}).AddEndpointFilter<AuthorizationActionFilter>();


app.Run();