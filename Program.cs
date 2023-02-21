using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/User", () => "Marcelo");
app.MapPost("/User", () => new { Name = "Marcelo", Age = 35 });
app.MapGet("/AddHeader", (HttpResponse response) =>
{
    response.Headers.Add("teste", "marcelo");
    return new { Name = "Victor", Age = 25 };
});

app.MapPost("/saveProduct", (Product product) =>
{
    return product.Code + " - " + product.Name;
});

app.MapGet("/getProduct/", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
{
    return dateStart + " - " + dateEnd;
});

app.MapGet("/getProduct/{code}", ([FromRoute] string code) =>
{
    return code;
});

app.MapGet("/getProductbyHeader", (HttpRequest request) =>
{
    return request.Headers["product-code"].ToString();
});

app.Run();

public class Product
{
    public string Code { get; set; }
    public string Name { get; set; }
}
