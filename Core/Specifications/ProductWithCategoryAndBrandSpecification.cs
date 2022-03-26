using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductWithCategoryAndBrandSpecification : BaseSpecification<Product>
    {
        public ProductWithCategoryAndBrandSpecification(ProductSpecificationParams productParams) : base
            (x =>
                (string.IsNullOrEmpty(productParams.Search) || x.Name.Contains(productParams.Search)) &&
                (!productParams.Brand.HasValue || x.BrandId == productParams.Brand) &&
                (!productParams.Category.HasValue || x.CategoryId == productParams.Category) &&
                (!productParams.Featured.HasValue || x.Featured == productParams.Featured )
            )
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.Brand);
            //AddInclude("NavA.NavB");
            AddInclude($"{nameof(Product.Variants)}");
            ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort)
                {
                    case "nameAsc":
                        AddOrder(p => p.Name);
                        break;
                    case "nameDesc":
                        AddOrderByDesc(p => p.Name);
                        break;
                    case "priceAsc":
                        /* AddOrder(p => p.Price); */
                        break;
                    case "priceDesc":
                        /* AddOrderByDesc(p => p.Price); */
                        break;
                    case "descriptionAsc":
                        AddOrder(p => p.Description);
                        break;
                    case "descriptionDesc":
                        AddOrderByDesc(p => p.Description);
                        break;
                    default:
                        AddOrder(p => p.Name);
                        break;
                }
            }

        }

        public ProductWithCategoryAndBrandSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.Brand);
            //AddInclude("Variants.ProductType");
            AddInclude($"{nameof(Product.Variants)}.{nameof(ProductType)}");
        }

    }
}
