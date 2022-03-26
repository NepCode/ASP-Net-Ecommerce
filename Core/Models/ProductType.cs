using System.ComponentModel.DataAnnotations.Schema;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ProductType : BaseClass
    {
        public string Name { get; set; } = string.Empty;
    }
}