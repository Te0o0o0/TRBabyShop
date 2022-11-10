﻿using Microsoft.EntityFrameworkCore;
using TRBabyShop.Core.Contracts;
using TRBabyShop.Core.Models;
using TRBabyShop.Infrastructure.Data;
using TRBabyShop.Infrastructure.Data.Common;
using TRBabyShop.Infrastructure.Data.Models;

namespace TRBabyShop.Core.Service
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext dbContext;

        private readonly IRepository repo;

        public ProductService(ApplicationDbContext _dbContext, IRepository _repo)
        {
            repo = _repo;
            dbContext = _dbContext;
        }

        public async Task<IEnumerable<ProductViewModel>> GetProductAsync()
        {
            var products = await dbContext.Products
                .ToListAsync();

            return products
                .Select(p => new ProductViewModel()
                {
                    Id=p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    Price = p.Price,
                    Image = p.Image,
                    Reviews = p.Reviews

                });
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await dbContext.Categories.ToListAsync();
        }

        public async Task<IEnumerable<ProductViewModel>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await dbContext.Products.ToListAsync();

            return products
                 .Where(p => p.CategoryId == categoryId)
                 .Select(p => new ProductViewModel()
                 {
                     CategoryId = p.CategoryId,
                     Name = p.Name,
                     Price = p.Price,
                     Image = p.Image,
                     Id = p.Id
                 });

        }

        public async Task AddProductAsync(AddProductViewModel model)
        {
            var newProduct = new Product()
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                Image = model.Image,
                CategoryId = model.CategoryId
            };

            await dbContext.Products.AddAsync(newProduct);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteProduct(int productId)
        {
            await repo.DeleteAsync<Product>(productId);
            await repo.SaveChangesAsync();
        }

        public async Task UpdateProduct(int productId, ProductViewModel model)
        {
            var product = await repo.GetByIdAsync<Product>(productId);
            if (product != null)
            {
                product.Id = model.Id;
                product.Name = model.Name;
                product.Price = model.Price;
                product.Category=model.Category;
                product.Description = model.Description;
                product.Image = model.Image;
                product.CategoryId=model.CategoryId;
                product.Reviews=model.Reviews;
            }

            throw new ArgumentException("Wrong product ID!");
            repo.Update(product);
            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductViewModel>> GetProductById(int productId)
        {
            var product = await dbContext.Products.ToListAsync();

            return product
                 .Where(p => p.Id == productId)
                 .Select(p => new ProductViewModel()
                 {
                     Id = p.Id,
                     Description=p.Description,
                     CategoryId = p.CategoryId,
                     Category=p.Category,
                     Name = p.Name,
                     Price = p.Price,
                     Image = p.Image,
                     Reviews=p.Reviews
                 });
        }
    }
}