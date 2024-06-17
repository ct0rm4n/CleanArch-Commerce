using Core.Entities.Domain.Blog;
using Core.Wrappers;
using Data.Interfaces;
using Data.Ioc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Hosting;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Service.Ioc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Core.ViewModel.Catalog;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureServices(x => x.AddAutofac()).UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacDataModule());
});
builder.Host.ConfigureServices(x => x.AddAutofac()).UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacPersistanceModule());
});

builder.Services.InjectConfigureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/blogpost/Get/{id}", Results<Ok<BlogPost>, NotFound> (IBlogPostService postService, int id) =>
        postService.Get(id) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound());

app.MapGet("/blogpost/GetALl",
    async (IBlogPostService postService, [FromQuery] int? PageNumber, [FromQuery] int? PageSize) =>
    {
        try
        {
            var paginationFilter = new PaginationFilter();
            if (PageNumber.HasValue && PageNumber > 0 && PageSize.HasValue && PageSize > 0)
                paginationFilter = new PaginationFilter((int)PageNumber, (int)PageSize);

            if ((PageNumber.HasValue && PageNumber > 0) && (PageSize == null || PageSize == 0))
                paginationFilter = new PaginationFilter((int)PageNumber, 10);

            var result = postService.Search(paginationFilter);

            if (result.Data is null || result.Data.Count == 0)
                return Results.NotFound("No blog posts found matching the filter.");

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    });

app.MapGet("/blogpost/Search", 
    async ([FromQuery] string filterText, [FromQuery] int? PageNumber,[FromQuery]int? PageSize,[FromServices] IBlogPostService postService) =>
{
    try
    {
        var paginationFilter = new PaginationFilter();
        if (PageNumber.HasValue && PageNumber > 0 && PageSize.HasValue && PageSize > 0)
            paginationFilter = new PaginationFilter((int)PageNumber, (int)PageSize, filterText);

        if ((PageNumber.HasValue && PageNumber > 0) && (PageSize == null || PageSize == 0))
            paginationFilter = new PaginationFilter((int)PageNumber, 10, filterText);

        if(!string.IsNullOrEmpty(filterText))
            paginationFilter.SerachText = filterText;

        var result = postService.Search(paginationFilter);
        if (result.Data is null || result.Data.Count == 0)
            return Results.NotFound("No blog posts found matching the filter.");

        return TypedResults.Ok(result);       
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.MapPost("/blogpost/Add", async (IBlogPostService postService, [FromBody] BlogPostVM body) =>
{
    try
    {
        
        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = postService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = postService.Add(JsonConvert.DeserializeObject<BlogPost>(serialized));
        return insert is not null 
        ? TypedResults.Ok(insert) 
        : TypedResults.NotFound();
    }
    catch(Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});


app.MapPost("/blogpost/update", async (IBlogPostService postService, [FromBody] BlogPostVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = postService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = postService.Update(JsonConvert.DeserializeObject<BlogPost>(serialized));
        return insert is not false
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.Run();


