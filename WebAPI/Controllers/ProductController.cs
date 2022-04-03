using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Product;
using WebAPI.Errors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/products")]
    [ApiController]
    public class ProductController : BaseAPIController
    {


        private readonly IGenericRepository<Product> __productRepository;
        private readonly IMapper _mapper;

        public ProductController(IGenericRepository<Product> productRepository, IMapper mapper )
        {
            this.__productRepository = productRepository;
            this._mapper = mapper;
        }


        // GET: api/<ProductController>
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<IReadOnlyList<ProductReadDTO>>>> GetProducts( [FromQuery] ProductSpecificationParams productParams)
        {
            //var products = await __productRepository.GetAllAsync();
            var specs = new ProductWithCategoryAndBrandSpecification(productParams);
            var products = await __productRepository.GetAllWithSpec(specs);

            var specCount = new ProductForCountingSpecification(productParams);
            var totalProducts = await __productRepository.CountAsync(specCount);

            var rounded = Math.Ceiling(Convert.ToDecimal(totalProducts / productParams.PageSize));
            var totalPages = Convert.ToInt32(rounded);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductReadDTO>>(products);
            return Ok(
                    new ServiceResponse<IReadOnlyList<ProductReadDTO>>
                    {
                        Count = totalProducts,
                        PageIndex = productParams.PageIndex,
                        PageSize = productParams.PageSize,
                        Data = data,
                        PageCount = totalPages
                    }
                );

            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductReadDTO>>(products));
            return Ok(products);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<ProductReadDTO>>> GetProductById(int id)
        {
            //var products = await __productRepository.GetByIdAsync(id);
            var specs = new ProductWithCategoryAndBrandSpecification(id);
            var product = await __productRepository.GetByIdWithSpec(specs);
            if (product == null) return NotFound(new CodeErrorResponse(404));
            return Ok(
                   new ServiceResponse<ProductReadDTO>
                   {
                       Data = _mapper.Map<Product, ProductReadDTO>(product)
                   }
               );
            /*return _mapper.Map<Product, ReadProductDTO>(product);
            return Ok(_mapper.Map<Product, ReadProductDTO>(product));
            return Ok(_mapper.Map<ReadProductDTO>(product));*/
        }

        // POST api/<ProductController>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(ProductCreateDTO product)
        {
            var productModel = _mapper.Map<ProductCreateDTO,Product>(product);
            var result = await __productRepository.Add(productModel);
            if (result == 0)
            {
                throw new Exception("there was an error inserting this product");
            }
            //return NotFound(new CodeErrorResponse(404, "product not found"));
            return Ok(productModel);
        }

        // PUT api/<ProductController>/5
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, ProductUpdateDTO product)
        {
            product.Id = id;
            var productModel = _mapper.Map<ProductUpdateDTO, Product>(product);
            var result = await __productRepository.Update(productModel);
            if (result == 0)
            {
                throw new Exception("there was an error inserting this product");
            }
            return Ok(product);
        }

        // DELETE api/<ProductController>/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteProduct(int id)
        {
            var result = await __productRepository.Delete(id);
            return Ok(result);
        }
    }
}
