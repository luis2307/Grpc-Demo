using Grpc.Core;
using GrpcProductService.Data;
using Microsoft.EntityFrameworkCore;

namespace GrpcProductService.Services
{
    public class ProductService : GrpcProductService.ProductService.ProductServiceBase
    {
        private readonly ProductContext _context;

        public ProductService(ProductContext context)
        {
            _context = context;
        }
        public override async Task<ProductList> GetProducts(Empty request, ServerCallContext context)
        {
            var products = await _context.Products.ToListAsync();
            var productList = new ProductList();
            productList.Products.AddRange(products.Select(p => new GrpcProductService.Product
            {
                Id = p.Id,
                Name = p.Name,
                Price = (double)p.Price
            }));

            return productList;
        }

        public override async Task<Product> GetProductById(ProductId request, ServerCallContext context)
        {
            var product = await _context.Products.FindAsync(request.Id);
            if (product == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
            }

            return new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = (double)product.Price
            };
        }

        public override async Task<ProductId> AddProduct(GrpcProductService.Product request, ServerCallContext context)
        {
            var product = new Product
            {
                Name = request.Name,
                Price = (double)request.Price
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ProductId { Id = product.Id };
        }

        public override async Task<Empty> DeleteProduct(ProductId request, ServerCallContext context)
        {
            var product = await _context.Products.FindAsync(request.Id);

            if (product == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(); 
            return new Empty();
        }
    }
}
