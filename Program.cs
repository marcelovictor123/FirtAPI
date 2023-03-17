using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapGet("/", () => "Hello World!");
app.MapGet("/User", () => "Marcelo");
app.MapPost("/User", () => new { Name = "Marcelo", Age = 35 });
app.MapGet("/AddHeader", (HttpResponse response) =>
{
    response.Headers.Add("teste", "marcelo");
    return new { Name = "Victor", Age = 25 };
});

app.MapPost("/products", (ProductDto productDto, ApplicationDbContext context) =>
{
    var category = context.Categories.Where(c => c.Id == productDto.CategoryId).First();
    var product = new Product
    {
        Code = productDto.Code,
        Name = productDto.Name,
        Description = productDto.Description,
        Category = category
    };
    if (productDto.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productDto.Tags)
        {
            product.Tags.Add(new Tag { Name = item });
        }
    }
    context.Products.Add(product);
    context.SaveChanges();
    return Results.Created("/products" + product.Id, product.Id);
});

app.MapGet("/products/", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
{
    return dateStart + " - " + dateEnd;
});

app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .Where(p => p.Id == id).First();
    if (product != null)
    {
        return Results.Ok(product);
    }
    else
    {
        return Results.NotFound();
    }

});

app.MapGet("/getProductbyHeader", (HttpRequest request) =>
{
    return request.Headers["product-code"].ToString();
});

app.MapPut("/products/{id}", ([FromRoute] int id, ApplicationDbContext context, ProductDto productDto) =>
{
    var product = context.Products
       .Include(p => p.Tags)
       .Where(p => p.Id == id).First();
    var category = context.Categories.Where(c => c.Id == productDto.CategoryId).First();

    product.Code = productDto.Code;
    product.Name = productDto.Name;
    product.Description = productDto.Description;
    product.Category = category;
    product.Tags = new List<Tag>();
    if (productDto.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productDto.Tags)
        {
            product.Tags.Add(new Tag { Name = item });
        }
    }

    context.SaveChanges();
    return Results.Ok();
});

app.MapDelete("/deleteproduct/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products.Where(p => p.Id == id).First();
    context.Products.Remove(product);
    return Results.Ok();
});

app.MapGet("/configuration/database", (IConfiguration configuration) =>
{
    return Results.Ok($"{configuration["database:Connection"]}/{configuration["database:Port"]}");
});

app.Run();
