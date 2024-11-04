using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Model;
using RabbitMQ.RabbitMQ;
using RabbitMQ.Services;

namespace RabbitMQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IRabbitMQProducer _rabbitMQProducer;

        public ProductController(IProductService productService, IRabbitMQProducer rabbitMQProducer)
        {
            _productService = productService;
            _rabbitMQProducer = rabbitMQProducer;
        }

        [HttpGet("product-list")]
        public IEnumerable<Product> ProductList()
        {
            var productList = _productService.GetProductList();
            return productList;
        }

        [HttpGet("get-product-by-id")]
        public ActionResult<Product> GetProductById(int Id)
        {
            var product = _productService.GetProductById(Id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        /// <summary>
        /// Add new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost("add-product")]
        public ActionResult<Product> AddProduct(Product product)
        {
            var productData = _productService.AddProduct(product);
            if (productData == null)
            {
                var errorMessage = $"Failed to add product: {product.ProductName}";

                _rabbitMQProducer.SendProductMessage(errorMessage, "CreateFail");

                return BadRequest(errorMessage);
            }

            _rabbitMQProducer.SendProductMessage(productData, "Create");

            return CreatedAtAction(nameof(GetProductById), new { Id = productData.ProductId }, productData);
        }

        [HttpPut("update-product")]
        public ActionResult<Product> UpdateProduct(Product product)
        {
            var productData = _productService.UpdateProduct(product);
            if (productData == null)
            {
                var errorMessage = $"Failed to update product: {product.ProductId}"; 

                _rabbitMQProducer.SendProductMessage(errorMessage, "UpdateFail");

                return NotFound("Product not found or failed to update."); 
            }

            _rabbitMQProducer.SendProductMessage(productData, "Update");

            return Ok(productData);
        }

        [HttpDelete("delete-product")]
        public ActionResult<bool> DeleteProduct(int Id)
        {
            var isSuccess = _productService.DeleteProduct(Id);

            if (!isSuccess)
            {
                var errorMessage = $"Failed to delete product with Id: {Id}";

                _rabbitMQProducer.SendProductMessage(errorMessage, "DeleteFail");

                return BadRequest(errorMessage);
            }
            else
            {
                var successMessage = $"Successfully deleted product with Id: {Id}";

                _rabbitMQProducer.SendProductMessage(successMessage, "Delete");

                return Ok(true);
            }
        }

        [HttpDelete("delete-all-product")]
        public ActionResult DeleteAllProduct()
        {
            var productList = _productService.GetProductList();

            foreach (var item in productList)
            {
                var isSuccess = _productService.DeleteProduct(item.ProductId);

                if (isSuccess)
                {
                    _rabbitMQProducer.SendProductMessage($"Successfully deleted product: {item.ProductName}", "Delete");
                }
                else
                {
                    _rabbitMQProducer.SendProductMessage($"Failed to delete product: {item.ProductName}", "DeleteFail");
                }
            }
            return NoContent(); // Return 204 for successful operation
        }
    }
}
