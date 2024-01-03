using Common.Models;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IPlaceItemRepository : IBaseRepository<PlaceItem>
    {
        public Task<PlaceItem> GetItemDescription(int placecItemId, string languageCode);
        public Task<PlaceItem> GetPlaceitemById(int placeItemId);
        public Task<PlaceItem> UpdatePlaceItemAsync(PlaceItem placeItem);
        public void DetachedPlaceInstance(PlaceItem placeItem);
    }
}
