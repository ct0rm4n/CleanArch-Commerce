using FastCommerce.Core.Entities.Domain.Blog;
using FastCommerce.Core.Wrappers;
using FastCommerce.Data.Interfaces;
using FastCommerce.Data.Ioc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Hosting;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using FastCommerce.Service.Ioc;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureServices(x => x.AddAutofac()).UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacDataModule());
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


app.Run();


