using Grpc.Net.Client;
using GrpcProductService;



// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("https://localhost:7202");
var client = new ProductService.ProductServiceClient(channel);

// Add a new product 
var newProduct = new Product
{
    Name = "Manzanas",
    Price = 5.99
};

var addedProductId = await client.AddProductAsync(newProduct);
Console.WriteLine($"Added Product ID: {addedProductId.Id}");


// Get all products
var products = await client.GetProductsAsync(new Empty());
Console.WriteLine("Products:");
foreach (var product in products.Products)
{
    Console.WriteLine($"- {product.Name} ({product.Price:C})");
}

// Get a product by ID
var productById = await client.GetProductByIdAsync(new ProductId { Id = addedProductId.Id });
Console.WriteLine($"Product with ID {addedProductId.Id}: {productById.Name} ({productById.Price:C})");

// Delete the product
await client.DeleteProductAsync(new ProductId { Id = addedProductId.Id });
Console.WriteLine($"Deleted Product with ID: {addedProductId.Id}");

// Verify the product is deleted
try
{
    var deletedProduct = await client.GetProductByIdAsync(new ProductId { Id = addedProductId.Id });
}
catch (Grpc.Core.RpcException ex) when (ex.Status.StatusCode == Grpc.Core.StatusCode.NotFound)
{
    Console.WriteLine("Product not found, as expected.");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey(); 