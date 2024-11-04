using Microsoft.EntityFrameworkCore;
using RabbitMQ.Data;
using RabbitMQ.Model;

namespace RabbitMQ.Services
{
    public class ProductService : IProductService
    {
        private readonly RabbitDbContext _dbContext;

        public ProductService(RabbitDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Product AddProduct(Product product)
        {
            var result = _dbContext.Products.Add(product);

            _dbContext.SaveChanges();

            return result.Entity;
        }

        public bool DeleteProduct(int Id)
        {
            var entity = _dbContext.Products.FirstOrDefault(x => x.ProductId == Id);

            if (entity == null)
            {
                return false;
            }

            var result = _dbContext.Products.Remove(entity);

            _dbContext.SaveChanges();

            return result != null ? true : false;
        }

        public Product GetProductById(int Id)
        {
            var entity = _dbContext.Products.AsNoTracking().FirstOrDefault(x => x.ProductId == Id);

            if (entity == null)
            {
                return null;
            }

            return entity;
        }

        public IEnumerable<Product> GetProductList()
        {
            return _dbContext.Products.ToList();
        }

        public Product UpdateProduct(Product product)
        {
            var entity = _dbContext.Products.FirstOrDefault(x => x.ProductId == product.ProductId); ;

            if (entity == null)
            {
                return null;
            }

            var result = _dbContext.Products.Update(product);

            _dbContext.SaveChanges();

            return result.Entity;
        }
    }
}
