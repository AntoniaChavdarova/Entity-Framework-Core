using RealEstates.Data;
using RealEstates.Models;
using RealEstates.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace RealEstates.Services
{
    public class PropertiesServices : IPropertiesServices
    {
        private RealEstateDbContext db;
        public PropertiesServices(RealEstateDbContext db)
        {
            this.db = db;
        }

        public void Create(string district, int price, int size, int? year, string buildingType, string propertyType, int? floor, int? maxFloor)
        {
            if (district == null)
            {
                throw new ArgumentNullException(nameof(district));
            }

            var property = new RealEstateProperty
            {
                Size = size,
                Price = price,
                Year = year < 1800 ? null : year,
                Floor = floor <= 0 ? null : floor,
                FloorMaxNumber = maxFloor <= 0 ? null : maxFloor
            };

            //District
            var districtEntity = this.db.Districts.FirstOrDefault(x => x.Name.Trim() == district.Trim());
            if(districtEntity == null)
            {
                districtEntity = new District { Name = district };
            }

            property.District = districtEntity;

            //PropertyType
            var propertyTypeEntity = this.db.PropertyTypes.FirstOrDefault(x => x.Name.Trim() == propertyType.Trim());
            if(propertyTypeEntity == null)
            {
                propertyTypeEntity = new PropertyType { Name = propertyType };
            }

            property.PropertyType = propertyTypeEntity;

            // Building Type
            var buildingTypeEntity = this.db.BuildingTypes.FirstOrDefault(x => x.Name.Trim() == buildingType.Trim());
            if (buildingTypeEntity == null)
            {
                buildingTypeEntity = new BuildingType { Name = buildingType };
            }

            property.BuildingType = buildingTypeEntity;

            this.db.RealEstateProperties.Add(property);

            this.db.SaveChanges();

            this.UpdateTags(property.Id);
        }

        public IEnumerable<PropertyViewModel> Search(int minPrice, int maxPrice, int minSize, int maxSize)
        {
            return db.RealEstateProperties
                .Where(x => x.Size > minSize && x.Size < maxSize && x.Price > minSize && x.Price < maxSize)
                .Select(MapToPropertViewModel())
                .OrderBy(x => x.Price)
                .ToList();
        }


        public IEnumerable<PropertyViewModel> SearchByPrice(int minPrice, int maxPrice)
        {
           return db.RealEstateProperties
                .Where(x => x.Price > minPrice && x.Price < maxPrice)
                .Select(MapToPropertViewModel())
                 .OrderBy(x => x.Price)
                .ToList();
        }

        public void UpdateTags(int propertyId)
        {
            var property = db.RealEstateProperties.FirstOrDefault(x => x.Id == propertyId);
            property.Tags.Clear();

            if(property.Year.HasValue && property.Year < 1990)
            {
                property.Tags.Add(
                    new RealEstatePropertyTag
                    {
                        Tag = this.GetOrCreateTag("OldBuilding")
                    }
                    );
            }

            if(((double)property.Price / property.Size) > 2000)
            {
                property.Tags.Add(
                new RealEstatePropertyTag
                {
                    Tag = this.GetOrCreateTag("Too expensive")
                });
            }

            if(property.Size > 120)
            {
                property.Tags.Add(
                    new RealEstatePropertyTag
                    {
                        Tag = this.GetOrCreateTag("Huge Apartment")
                    });
            }

            if (property.Year > 2018 && property.FloorMaxNumber > 5)
            {
                property.Tags.Add(
                    new RealEstatePropertyTag
                    {
                        Tag = this.GetOrCreateTag("HasParking")
                    });
            }

            if (property.Floor == property.FloorMaxNumber)
            {
                property.Tags.Add(
                    new RealEstatePropertyTag
                    {
                        Tag = this.GetOrCreateTag("LastFloor")
                    });
            }

            if (((double)property.Price / property.Size) < 800)
            {
                property.Tags.Add(
                    new RealEstatePropertyTag
                    {
                        Tag = this.GetOrCreateTag("CheapProperty")
                    });
            }

            this.db.SaveChanges();

           
        }

        private Tag GetOrCreateTag(string tag)
        {
            var tagEntity = this.db.Tags.FirstOrDefault(x => x.Name.Trim() == tag.Trim());
            if (tagEntity == null)
            {
                tagEntity = new Tag { Name = tag };
            }

            return tagEntity;
        }

        private static Expression<Func<RealEstateProperty, PropertyViewModel>> MapToPropertViewModel()
        {
            return x => new PropertyViewModel
            {
                District = x.District.Name,
                BuildingType = x.BuildingType.Name,
                Floor = (x.Floor ?? 0).ToString()
                                   + "/"
                                   + (x.FloorMaxNumber ?? 0),
                Price = x.Price,
                Size = x.Size,
                Year = x.Year,
                PropertyType = x.PropertyType.Name

            };
        }
    }
}
