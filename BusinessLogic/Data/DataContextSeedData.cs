using Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Data
{
    public class DataContextSeedData
    {
        public static async Task LoadDataAsync(DataContext context, ILoggerFactory loggerFactory)
        {

            try
            {
                if (!context.ProductTypes.Any())
                {
                    var productTypeData = File.ReadAllText("../BusinessLogic/DataInitializer/productType.json");
                    var ProductTypes = JsonSerializer.Deserialize<List<ProductType>>(productTypeData);

                    foreach (var productType in ProductTypes)
                    {
                        context.ProductTypes.Add(productType);
                    }

                    await context.SaveChangesAsync();
                }

                if (!context.Brands.Any())
                {
                    var brandData = File.ReadAllText("../BusinessLogic/DataInitializer/brand.json");
                    var brands = JsonSerializer.Deserialize<List<Brand>>(brandData);

                    foreach (var brand in brands)
                    {
                        context.Brands.Add(brand);
                    }

                    await context.SaveChangesAsync();
                }

                if (!context.Categories.Any())
                {
                    var categoryData = File.ReadAllText("../BusinessLogic/DataInitializer/category.json");
                    var categories = JsonSerializer.Deserialize<List<Category>>(categoryData);

                    foreach (var category in categories)
                    {
                        context.Categories.Add(category);
                    }

                    await context.SaveChangesAsync();
                }

                if (!context.Products.Any())
                {
                    var productData = File.ReadAllText("../BusinessLogic/DataInitializer/product.json");
                    var products = JsonSerializer.Deserialize<List<Product>>(productData);

                    foreach (var product in products)
                    {
                        context.Products.Add(product);
                    }

                    await context.SaveChangesAsync();
                }

                if (!context.ProductVariants.Any())
                {
                    var productVariantsData = File.ReadAllText("../BusinessLogic/DataInitializer/productVariant.json");
                    var productVariants = JsonSerializer.Deserialize<List<ProductVariant>>(productVariantsData);

                     foreach (var productVariant in productVariants)
                    {
                        context.ProductVariants.Add(productVariant);
                    } 
                  
                    await context.SaveChangesAsync();
                }

                /*if (!context.ShippingType.Any())
                {
                    var shippingTypeData = File.ReadAllText("../BusinessLogic/DataInitializer/shippingtype.json");
                    var shippingTypes = JsonSerializer.Deserialize<List<ShippingType>>(shippingTypeData);

                    foreach (var shippingType in shippingTypes)
                    {
                        context.ShippingType.Add(shippingType);
                    }

                    await context.SaveChangesAsync();
                }*/

            }
            catch (Exception e)
            {

                var logger = loggerFactory.CreateLogger<DataContext>();
                logger.LogError(e.Message);

            }
        }
    }
}
