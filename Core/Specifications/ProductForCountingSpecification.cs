using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductForCountingSpecification : BaseSpecification<Product>
    {
        public ProductForCountingSpecification(ProductSpecificationParams productParams)
            : base(x =>
                (string.IsNullOrEmpty(productParams.Search) || x.Name.Contains(productParams.Search)) &&
                (!productParams.Brand.HasValue || x.BrandId == productParams.Brand) &&
                (!productParams.Category.HasValue || x.CategoryId == productParams.Category) &&
                (!productParams.Featured.HasValue || x.Featured == productParams.Featured )
            )
        { }
    }
}
