using BusinessLogic.Data;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        public ProductRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyList<Product>> GetProducsAsync()
        {
            return await _context.Products
                            .Include(p => p.Brand)
                            .Include(p => p.Category)
                            .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                            .Include(p => p.Brand)
                            .Include(p => p.Category)
                            .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
