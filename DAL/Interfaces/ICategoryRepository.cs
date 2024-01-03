using Common.Models;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<List<CategoryLanguage>> GetCategories(string languageCode);
        Task<bool> ChangeStatusAsync(int categoryId);
        Task<Category> GetDetailAsync(int categoryId);
        Task<List<Category>> GetAllWithoutPagingAsync();
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> CategoryNameIsExist(string categoryName);
        Task<bool> CategoryIsExist(int categoryId);
        Task<bool> CategoryNameIsExist(string categoryName, int categoryId);
        Task<PagedResult<T>> GetListAsync<T>(QueryParameters queryParameters, string cacheKey);
    }
}
