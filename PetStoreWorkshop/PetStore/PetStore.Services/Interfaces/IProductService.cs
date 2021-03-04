using System;
using System.Collections.Generic;
using System.Text;

using PetStore.ServiceModels.Products.InputModels;
using PetStore.ServiceModels.Products.OutputModels;

namespace PetStore.Services.Interfaces
{
    public interface IProductService
    {
        void AddProduct(AddProductInputServiceModel model);
        bool RemoveById(string id);

        bool RemoveByName(string name);

        void EditProduct(string id , EditProductInputServiceModel);

        ICollection<ListAllProductServiceModel> GetAll();

        ICollection<ListAllProductsByProductTypeServiceModel> ListAllByProductType(string type);

        ICollection<ListAllProductsByNameServiceModel> SearchByName(string searchStr , bool caseSensitive);





    }
}
