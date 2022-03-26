using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductSpecificationParams
    {
        public int? Brand { set; get; }
        public int? Category { set; get; }
        public string? Sort { set; get; }
        public bool? Featured { set; get; }
        public int PageIndex { set; get; } = 1;

        private const int MaxPageSize = 50;

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? Search { set; get; }
    }
}
