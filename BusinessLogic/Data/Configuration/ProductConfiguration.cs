using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
            builder.Property(p => p.ImageUrl).IsRequired().HasMaxLength(500);
            /* builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)"); */
            builder.HasOne(p => p.Brand).WithMany().HasForeignKey( b => b.BrandId );
            builder.HasOne(p => p.Category).WithMany().HasForeignKey( c => c.CategoryId);
        }
    }
}
