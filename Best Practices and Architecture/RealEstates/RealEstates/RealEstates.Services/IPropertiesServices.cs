using RealEstates.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstates.Services
{
    public interface IPropertiesServices
    {
        void Create(string district, int price, int size, int? year, string buildingType, string propertyType, int? floor, int? maxFloor);

        void UpdateTags(int propertyId);

        IEnumerable<PropertyViewModel> Search(int minPrice, int maxPrice, int minSize, int maxSize);

        IEnumerable<PropertyViewModel> SearchByPrice(int minPrice, int maxPrice);
    }
}
