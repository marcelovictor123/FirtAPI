using Microsoft.AspNetCore.Mvc;

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
    context.Products.Add(product);
    context.SaveChanges();
    return Results.Created("/products" + product.Id, product.Id);
});

app.MapGet("/products/", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
{
    return dateStart + " - " + dateEnd;
});

app.MapGet("/products/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.GetBy(code);
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

app.MapPut("/products", (Product product) =>
{
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    ProductRepository.Add(productSaved); // save changes to repository
    return new { Message = "Product updated successfully" }; // return a response
});

app.MapDelete("/deleteproduct/{code}", ([FromRoute] string code) =>
{
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Remove(productSaved);
    return Results.Ok();
});

app.MapGet("/configuration/database", (IConfiguration configuration) =>
{
    return Results.Ok($"{configuration["database:Connection"]}/{configuration["database:Port"]}");
});

app.Run();
