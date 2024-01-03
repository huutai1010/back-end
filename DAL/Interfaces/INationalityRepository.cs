using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface INationalityRepository : IBaseRepository<Nationality>
    {
        Task<bool> ChangeStatusNationalityByLanguageCode(string languageCode);
        Task<bool> NationCodeIsExist(string nationalCode);
        Task<List<Nationality>> GetListNationalityAsync();
    }
}
