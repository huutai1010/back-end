using AutoMapper;
using Common.Interfaces;
using Common.Models;
using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
        }

        public async Task<PagedResult<T>> GetListAsync<T>(QueryParameters queryParameters, string cacheKey)
        {
            List<Category>? items = await _redisCacheService.Get<List<Category>>(cacheKey);
            if (items is null)
            {
                items = await _context.Categories.Include(x => x.CategoryLanguages).IgnoreQueryFilters().ToListAsync();
                await _redisCacheService.SaveCacheAsync(cacheKey, items);
            }

            var result = _mapper.Map<List<T>>(items);

            string? searchByPropName = queryParameters.SearchBy;
            string? searchText = queryParameters.Search;
            if (!string.IsNullOrEmpty(searchByPropName) && !string.IsNullOrEmpty(searchText))
            {
                var searchProp = typeof(T).GetProperty(searchByPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var search = queryParameters.Search?.ToLower();
                if (searchProp != null && !string.IsNullOrEmpty(search))
                {
                    result = result.Where(x =>
                        searchProp.GetValue(x, null)
                        ?.ToString()?.ToLower()
                        .Contains(search) ?? true)
                        .ToList();
                }
            }

            var totalSize = items.Count;
            var pageCount = (double)totalSize / queryParameters.PageSize;

            return new PagedResult<T>
            {
                Data = result
                        .Skip((queryParameters.PageNumber) * queryParameters.PageSize)
                        .Take(queryParameters.PageSize).ToList(),
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                PageCount = (int)Math.Ceiling(pageCount),
                TotalCount = totalSize,
            };
        }

        public async Task<List<Category>> GetAllWithoutPagingAsync()
        {
            var categories = await _context.Categories.Where(x => x.Status != 0).ToListAsync();
            return categories;
        }

        public async Task<List<CategoryLanguage>> GetCategories(string languageCode)
        {
            return await  _context.CategoryLanguages
              .Where(x => x.LanguageCode == languageCode)
              .ToListAsync();
        }

        public async Task<bool> ChangeStatusAsync(int categoryId)
        {
            var category = await _context.Categories
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == categoryId);
            if (category == null)
            {
                return false;
            }
            else if (category.Status == 1)
            {
                category.Status = 0;
            }
            else
            {
                category.Status = 1;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Category> GetDetailAsync(int categoryId)
        {
            var category = await _context.Categories
                .IgnoreQueryFilters()
                .Include(x => x.CategoryLanguages)
                .SingleOrDefaultAsync(x => x.Id == categoryId);

            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category categoryUpdate)
        {
            var category = new Category();
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var categoryLanguages = _context.CategoryLanguages.IgnoreQueryFilters().Where(x => x.CategoryId == categoryUpdate.Id).ToList();
                    if (categoryLanguages != null)
                    {
                        _context.CategoryLanguages.RemoveRange(categoryLanguages);
                    }
                    await _context.CategoryLanguages.AddRangeAsync(categoryUpdate.CategoryLanguages);

                    category = _context.Categories.SingleOrDefaultAsync(x => x.Id == categoryUpdate.Id).Result;

                    category.UpdateTime = categoryUpdate.UpdateTime;
                    category.Name= categoryUpdate.Name;
                    _context.Entry(category).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
            return category;
        }

        public async Task<bool> CategoryNameIsExist(string categoryName)
        {
            var category = await _context.Categories.IgnoreQueryFilters().SingleOrDefaultAsync(c => c.Name == categoryName);
            if(category == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CategoryNameIsExist(string categoryName, int categoryId)
        {
            var category = await _context.Categories.IgnoreQueryFilters().SingleOrDefaultAsync(c => c.Name == categoryName && c.Id != categoryId);
            if (category == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CategoryIsExist(int categoryId)
        {
            var category = await _context.Categories.IgnoreQueryFilters().SingleOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                return false;
            }
            return true;
        }
    }
}
