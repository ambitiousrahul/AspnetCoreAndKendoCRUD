using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Common;
using PracticeWeb.Data;
using PracticeWeb.Models;
using PracticeWeb.Pages;
using PracticeWeb.ViewModel;

namespace PracticeWeb.RESTAPIEndpoints
{
    [Route("api/")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly PracticeWebDBContext _context;

        public ProductsController(PracticeWebDBContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        [Route("products")]
        [ServiceFilter(typeof(ValidateActionParametersAttribute))]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProducts()
        {
            return await _context.Product.
                                Join(_context.ProductCategory,
                                pr => pr.CategoryId,
                                pc => pc.Id,
                                (pr, pc) => new ProductViewModel
                                {
                                    ProductCode = pr.ProductCode,
                                    Name = pr.Name,
                                    CategoryName = pc.Name,
                                    UnitPrice =pr.UnitPrice ,
                                    ManufactureDate = pr.ManufactureDate.ToShortDateString()

                                })?.ToListAsync();

        }

        // GET: api/Products/5
        [HttpGet]
        [Route("products/{id}")]
        public async Task<ActionResult<ProductViewModel>> GetProduct(int id)
        {
            var result = await _context.Product.Where(pr => pr.ProductCode == id)?
                              .Join(_context.ProductCategory,
                              pr => pr.CategoryId,
                              pc => pc.Id,
                              (pr, pc) => new ProductViewModel
                              {
                                  ProductCode = pr.ProductCode,
                                  Name = pr.Name,
                                  CategoryName = pc.Name,
                                  UnitPrice = pr.UnitPrice,
                                  ManufactureDate = pr.ManufactureDate.ToShortDateString()

                              }).FirstOrDefaultAsync();
        

            if (result is null)
            {
                return NotFound();
            }
            return result;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("products")]
        [ServiceFilter(typeof(ValidateActionParametersAttribute))]
        public async Task<IActionResult> PutProduct([FromBody] List<ProductUpdateModel> productsToBeUpdated)
        {
            if (productsToBeUpdated.Any(p => (p.ProductCode is null) || (p.ProductCode == 0)))
            {
                return BadRequest();
            }
            
            IList<Product> products = new List<Product>(productsToBeUpdated.Count);
            
            foreach (ProductUpdateModel p in productsToBeUpdated)
            {
                var product = await InstantiateProductAsync(p, false);

                _context.Entry(product).State = EntityState.Modified;

                products.Add(product);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
                
            }

            return Ok();
        }

        // POST: api/Products
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Route("products")]
        [ServiceFilter(typeof(ValidateActionParametersAttribute))]
        public async Task<ActionResult<Product>> PostProduct([FromBody]List<ProductCreateModel> productsToBeCreated)
        {
            //List<string> paramProductCategoryNames = productsToBeUpdated.Select(pr => pr.CategoryName).ToHashSet().ToList();
            IList<Product> products = new List<Product>(productsToBeCreated.Count);

            //var categoryIds = await _context.ProductCategory.Where(pr => paramProductCategoryNames.Contains(pr.Name)).Select(pr => pr.Id).ToListAsync();

            try
            {
                foreach (ProductCreateModel p in productsToBeCreated)
                {
                    var newProduct = await InstantiateProductAsync(p);

                    _context.Entry(newProduct).State = EntityState.Modified;

                    products.Add(newProduct);
                }

                _context.Product.AddRange(products);
                await _context.SaveChangesAsync();

                foreach (Product p in products)
                {
                    ProductCreateModel pr = productsToBeCreated.FirstOrDefault(pr => pr.Name.Equals(p.Name));
                    pr.ProductCode = p.ProductCode;
                    pr.AddedDate = p.AddedDate.ToShortDateString();
                    pr.ManufactureDate = p.ManufactureDate.ToShortDateString();
                    pr.CategoryName = p.Category.Name;
                    pr.LastEditedDate = p.LastEditedDate.ToShortDateString();
                }

                return CreatedAtAction("GetProducts", productsToBeCreated);
            }
            catch (DbUpdateException)
            {
                return Conflict(new ApiErrorResponse { StatusCode = StatusCodes.Status409Conflict, Message = "UNIQUE KEY Constraint violated for product name" });
            }

        }

        // DELETE: api/Products/5
        [HttpDelete("products/{id}")]
        [ServiceFilter(typeof(ValidateActionParametersAttribute))]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }
       

        private async Task<Product> InstantiateProductAsync<ModelEntity>(ModelEntity prEditModel,bool isNewRecord=true) where ModelEntity : ProductCreateModel
        {
            Product product = isNewRecord ? new Product() : await _context.Product.FindAsync(prEditModel.ProductCode);            
            
            if(product is null)
            {
                throw new KeyNotFoundException("The given product code was not present in the server");
            }
                     
            product.Name = prEditModel.Name;
            product.LastEditedDate = DateTime.Now;
            product.UnitPrice = prEditModel.UnitPrice;

            if (isNewRecord)
            {
                //bool isCorrectDateTimeFormat = DateTime.TryParse(prEditModel.AddedDate, out DateTime productAddedDate);
                bool isCorrectManufactureDateFormat = DateTime.TryParse(prEditModel.ManufactureDate, out DateTime productManufactureDate);

                product.IsActive = true;
                product.AddedDate = DateTime.Now;
                product.ManufactureDate = isCorrectManufactureDateFormat ? productManufactureDate : throw new InvalidCastException("Specified ManufactureDate property value was not in  proper date format");                
            }
            ProductCategory pCategory = await _context.ProductCategory.FirstOrDefaultAsync(pr => pr.Name.Equals(prEditModel.CategoryName));

            if(pCategory is null)
            {
                throw new KeyNotFoundException("The passed product category was not present in the database");
            }
            product.CategoryId = pCategory.Id;
            return product;
        }
    }
}
