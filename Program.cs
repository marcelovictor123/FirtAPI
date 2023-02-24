using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
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

app.MapPost("/products", (Product product) =>
{
    ProductRepository.Add(product);
    return Results.Created("/products" + product.Code, product.Code);
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



public static class ProductRepository
{
    public static List<Product> Products { get; set; } = Products = new List<Product>();

    public static void Init(IConfiguration configuration)
    {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product)
    {
        Products.Add(product);
    }

    public static Product GetBy(string code)
    {
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product)
    {
        Products.Remove(product);
    }
}

public class Product
{
    public string Code { get; set; }
    public string Name { get; set; }
}
