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
using Service.Interfaces;
using Autofac.Core;
using Data.Commands;
using Service.Helpers;
using Azure.Core.GeoJson;


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
    
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog Web Api", Version = "v1" });
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
builder.Services.AddSingleton<AuthorizationActionFilter>();

var app = builder.Build();

var databaseVerify = new DatabaseStarter(app.Configuration.GetConnectionString("SqlConnection"));

// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog Web Api"));


app.UseHttpsRedirection();

app.MapGet("api/catalog", Results<Ok<BlogPost>, NotFound> (IBlogPostService postService, int id) =>
        postService.Get(1) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound()).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("api/Catalog/blogpost/Get/{id}", Results<Ok<BlogPost>, NotFound> (IBlogPostService postService, int id) =>
        postService.Get(id) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound()).AddEndpointFilter<AuthorizationActionFilter>();



app.MapGet("/api/Catalog/blogpost/GetALl", async (IBlogPostService postService, [FromQuery] int? PageNumber, [FromQuery] int? PageSize) =>
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

app.MapGet("/api/Catalog/blogpost/Search", 
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

app.MapPost("/api/Catalog/blogpost/Add", async (IBlogPostService postService, [FromBody] BlogPostVM body) =>
{
    try
    {
        

        var serialized = JsonConvert.SerializeObject(body, new JsonSerializerSettings()
        {
            ContractResolver = new IgnorePropertiesResolver(new[] { "Id" })
        });

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


app.MapPost("/api/Catalog/blogpost/update", async (IBlogPostService postService, [FromBody] BlogPostVM body) =>
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

app.MapPost("/api/Catalog/blogpost/massInsert", async (IBlogPostService postService) =>
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
app.MapGet("/api/Catalog/Category/Get/{id}", Results<Ok<Category>, NotFound> (ICategoryService cService, int id) =>
        cService.Get(id) is { } category
            ? TypedResults.Ok(category)
            : TypedResults.NotFound());

app.MapGet("/api/Catalog/Category/GetALl",
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

app.MapGet("/api/Catalog/Category/Search",
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

app.MapPost("/api/Catalog/Category/Add", async (ICategoryService cService, [FromBody] CategoryVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body, new JsonSerializerSettings()
        {
            ContractResolver = new IgnorePropertiesResolver(new[] { "Id" })
        });
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


app.MapPost("/api/Catalog/Category/update", async (ICategoryService cService, [FromBody] CategoryVM body) =>
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
app.MapGet("/api/Catalog/Settings/Get/{id}", Results<Ok<Settings>, NotFound> (ISettingsService setService, int id) =>
        setService.Get(id) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound()).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("/api/Catalog/Settings/GetALl",
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

app.MapGet("/api/Catalog/Settings/Search",
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

app.MapPost("/api/Catalog/Settings/Add", async (ISettingsService setService, [FromBody] SettingsVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body, new JsonSerializerSettings()
        {
            ContractResolver = new IgnorePropertiesResolver(new[] { "Id" })
        });
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


app.MapPost("/api/Catalog/Settings/update", async (ISettingsService setService, [FromBody] SettingsVM body) =>
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
app.MapGet("/api/Catalog/Banner/Get/{id}", Results<Ok<Banner>, NotFound> (IBannerService setService, int id) =>
        setService.Get(id) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound()).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("/api/Catalog/Banner/GetALl",
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

app.MapGet("/api/Catalog/Banner/Search",
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

app.MapPost("/api/Catalog/Banner/Add", async (IBannerService setService, [FromBody] BannerVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body, new JsonSerializerSettings()
        {
            ContractResolver = new IgnorePropertiesResolver(new[] { "Id" })
        });
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


app.MapPost("/api/Catalog/Banner/update", async (IBannerService setService, [FromBody] BannerVM body) =>
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


/*product*/
app.MapGet("/api/Catalog/product/Get/{id}", Results<Ok<Product>, NotFound> (IProductService setService, int id) =>
        setService.Get(id) is { } post
            ? TypedResults.Ok(post)
            : TypedResults.NotFound()).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("/api/Catalog/product/Getall",
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
    }).AddEndpointFilter<AuthorizationActionFilter>();

app.MapGet("/api/Catalog/Product/Search",
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
    }).AddEndpointFilter<AuthorizationActionFilter>();

app.MapPost("/api/Catalog/Product/Add", async (IProductService setService, [FromBody] ProductVM body) =>
{
    try
    {

        var serialized = JsonConvert.SerializeObject(body, new JsonSerializerSettings()
        {
            ContractResolver = new IgnorePropertiesResolver(new[] { "Id" })
        });
        List<string> validation = setService.GetValidation(body).ToList();
        if (validation is not null && validation.Count() > 0)
            return TypedResults.Problem(JsonConvert.SerializeObject(validation));

        var insert = setService.Add(JsonConvert.DeserializeObject<Product>(serialized));
        //if(insert is not null && insert.Id > 0)
        //    var insert_images = setService.AddImages(JsonConvert.DeserializeObject<ProductImage>(body.Image));
        return insert is not null
        ? TypedResults.Ok(insert)
        : TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}).AddEndpointFilter<AuthorizationActionFilter>();


app.MapPost("/api/Catalog/Product/update", async (IProductService setService, [FromBody] ProductVM body) =>
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
}).AddEndpointFilter<AuthorizationActionFilter>();
//login


app.MapPost("/api/login", async (IAuthService setService, [FromBody] LoginVM body) =>
{
    try
    {
        var (token, logged) = setService.Login(body.Email, body.Password);
        return TypedResults.Ok(new { Token = token, Logged = logged });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});
app.MapGet("/api/loginvalidate", async (IAuthService setService, [FromQuery] string token) =>
{
    try
    {
        var (token_valid, logged) = setService.LoginValidate(token);
        return TypedResults.Ok(new { Token = token_valid, Logged = logged });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});


app.Run();


