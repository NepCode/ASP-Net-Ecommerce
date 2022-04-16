using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data.Configuration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(oi => oi.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.HasKey(oi => new { oi.OrderId, oi.ProductId, oi.ProductTypeId });
        }
    }
}
