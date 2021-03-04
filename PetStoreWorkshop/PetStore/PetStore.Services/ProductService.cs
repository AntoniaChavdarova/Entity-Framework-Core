using AutoMapper;
using PetStore.ServiceModels.Products.InputModels;
using PetStore.ServiceModels.Products.OutputModels;
using PetStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper mapper;


        public void AddProduct(AddProductInputServiceModel model)
        {
            throw new NotImplementedException();
        }

        public void EditProduct(string id, EditProductInputServiceModel )
        {
            throw new NotImplementedException();
        }

        public ICollection<ListAllProductServiceModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public ICollection<ListAllProductsByProductTypeServiceModel> ListAllByProductType(string type)
        {
            throw new NotImplementedException();
        }

        public bool RemoveById(string id)
        {
            throw new NotImplementedException();
        }

        public bool RemoveByName(string name)
        {
            throw new NotImplementedException();
        }

        public ICollection<ListAllProductsByNameServiceModel> SearchByName(string searchStr, bool caseSensitive)
        {
            throw new NotImplementedException();
        }
    }
}
