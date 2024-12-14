using Core.Entities.Domain.Blog;
using Core.Wrappers;
using Data.Interfaces;
using Data.Ioc;
using Microsoft.AspNetCore.Http.HttpResults;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Service.Ioc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Core.ViewModel.Catalog;
using Core.Entities.Domain.Catalog;
using Core.Entities.Enum;
using Core.Entities.Abstract;
using Core.ViewModel.Generic.Abstracts;
using Core.ViewModel.Banner;
using Core.Entities.Domain.Banner;
using Core.Entities.Domain.User;
using Core.ViewModel.User;
using Core.Entities.Domain;
using Microsoft.OpenApi.Models;
using Service.Filter;
using Microsoft.AspNetCore.Authorization;


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

builder.Services.InjectConfigureServices();
builder.Services.AddSingleton<AuthorizationActionFilter>();

var app = builder.Build();
var auth = app.Configuration;

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



app.MapGet("/blogpost/GetALl", async (IBlogPostService postService, [FromQuery] int? PageNumber, [FromQuery] int? PageSize) =>
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
    }).AddEndpointFilter<AuthorizationActionFilter>();

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
}).AddEndpointFilter<AuthorizationActionFilter>();

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
}).AddEndpointFilter<AuthorizationActionFilter>();


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
}).AddEndpointFilter<AuthorizationActionFilter>();

app.MapPost("/blogpost/massInsert", async (IBlogPostService postService) =>
{
    try
    {
        IList<BlogPost> blogPosts = new List<BlogPost>();
        for (int i = 0; i < 1000; i++)
        {
            blogPosts.Add(new BlogPost
            {
                Name = $"Post {i}",
                Html = $"<div><div><b>Chorume guid {Guid.NewGuid().ToString()}</b></div></div>",
                Status = StatusEntity.Inserted
            });
        }        
        var bookList  = postService.BulkInsertReturn(blogPosts);

        return bookList is not EmptyResult
        ? TypedResults.Ok(bookList)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}).AddEndpointFilter<AuthorizationActionFilter>();


/*Catgeory*/
app.MapGet("/Category/Get/{id}", Results<Ok<Category>, NotFound> (ICategoryService cService, int id) =>
        cService.Get(id) is { } category
            ? TypedResults.Ok(category)
            : TypedResults.NotFound());

app.MapGet("/Category/GetALl",
    async (ICategoryService cService, [FromQuery] int? PageNumber, [FromQuery] int? PageSize) =>
    {
        try
        {
            var paginationFilter = new PaginationFilter();
            if (PageNumber.HasValue && PageNumber > 0 && PageSize.HasValue && PageSize > 0)
                paginationFilter = new PaginationFilter((int)PageNumber, (int)PageSize);

            if ((PageNumber.HasValue && PageNumber > 0) && (PageSize == null || PageSize == 0))
                paginationFilter = new PaginationFilter((int)PageNumber, 10);

            var result = cService.Search(paginationFilter);

            if (result.Data is null || result.Data.Count == 0)
                return Results.NotFound("No items found matching the filter.");

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }).AddEndpointFilter<AuthorizationActionFilter>(); 

app.MapGet("/Category/Search",
    async ([FromQuery] string filterText, [FromQuery] int? PageNumber, [FromQuery] int? PageSize, [FromServices] ICategoryService cService) =>
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

            var result = cService.Search(paginationFilter);
            if (result.Data is null || result.Data.Count == 0)
                return Results.NotFound("No blog posts found matching the filter.");

            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }).AddEndpointFilter<AuthorizationActionFilter>(); 

app.MapPost("/Category/Add", async (ICategoryService cService, [FromBody] CategoryVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = cService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = cService.Add(JsonConvert.DeserializeObject<Category>(serialized));
        return insert is not null
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}).AddEndpointFilter<AuthorizationActionFilter>();


app.MapPost("/Category/update", async (ICategoryService cService, [FromBody] CategoryVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = cService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = cService.Update(JsonConvert.DeserializeObject<Category>(serialized));
        return insert is not false
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}).AddEndpointFilter<AuthorizationActionFilter>();

/*Settings*/
app.MapGet("/Settings/Get/{id}", Results<Ok<Settings>, NotFound> (ISettingsService setService, int id) =>
        setService.Get(id) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound()).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("/Settings/GetALl",
    async (ISettingsService setService, [FromQuery] int? PageNumber, [FromQuery] int? PageSize) =>
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
                return Results.NotFound("No blog posts found matching the filter.");

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("/Settings/Search",
    async ([FromQuery] string filterText, [FromQuery] int? PageNumber, [FromQuery] int? PageSize, [FromServices] ISettingsService setService) =>
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

app.MapPost("/Settings/Add", async (ISettingsService setService, [FromBody] SettingsVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = setService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = setService.Add(JsonConvert.DeserializeObject<Settings>(serialized));
        return insert is not null
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}).AddEndpointFilter<AuthorizationActionFilter>();


app.MapPost("/Settings/update", async (ISettingsService setService, [FromBody] SettingsVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = setService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = setService.Update(JsonConvert.DeserializeObject<Settings>(serialized));
        return insert is not false
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}).AddEndpointFilter<AuthorizationActionFilter>();

/*Banner*/
app.MapGet("/Banner/Get/{id}", Results<Ok<Banner>, NotFound> (IBannerService setService, int id) =>
        setService.Get(id) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound()).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("/Banner/GetALl",
    async (IBannerService setService, [FromQuery] int? PageNumber, [FromQuery] int? PageSize) =>
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
                return Results.NotFound("No blog posts found matching the filter.");

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("/Banner/Search",
    async ([FromQuery] string filterText, [FromQuery] int? PageNumber, [FromQuery] int? PageSize, [FromServices] IBannerService setService) =>
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
    });

app.MapPost("/Banner/Add", async (IBannerService setService, [FromBody] BannerVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = setService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = setService.Add(JsonConvert.DeserializeObject<Banner>(serialized));
        return insert is not null
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});


app.MapPost("/Banner/update", async (IBannerService setService, [FromBody] BannerVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = setService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = setService.Update(JsonConvert.DeserializeObject<Banner>(serialized));
        return insert is not false
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});


/*role*/
app.MapGet("/Role/Get/{id}", Results<Ok<Role>, NotFound> (IRoleService setService, int id) =>
        setService.Get(id) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound());

app.MapGet("/Role/GetALl",
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
    });

app.MapGet("/Role/Search",
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
    });

app.MapPost("/Role/Add", async (IRoleService setService, [FromBody] RoleVM body) =>
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
});


app.MapPost("/Role/update", async (IRoleService setService, [FromBody] RoleVM body) =>
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
});
/*product*/
app.MapGet("/product/Get/{id}", Results<Ok<Product>, NotFound> (IProductService setService, int id) =>
        setService.Get(id) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound());

app.MapGet("/product/Getall",
    async (IProductService setService, [FromQuery] int? PageNumber, [FromQuery] int? PageSize) =>
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
    });

app.MapGet("/Product/Search",
    async ([FromQuery] string filterText, [FromQuery] int? PageNumber, [FromQuery] int? PageSize, [FromServices] IProductService setService) =>
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
    });

app.MapPost("/Product/Add", async (IProductService setService, [FromBody] ProductVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = setService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = setService.Add(JsonConvert.DeserializeObject<Product>(serialized));
        return insert is not null
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});


app.MapPost("/Product/update", async (IProductService setService, [FromBody] ProductVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body);
        List<string> validation = setService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = setService.Update(JsonConvert.DeserializeObject<Product>(serialized));
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


