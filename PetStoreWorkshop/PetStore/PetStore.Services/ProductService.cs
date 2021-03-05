using AutoMapper;
using PetStore.Common;
using PetStore.Data;
using PetStore.Models;
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
        private readonly PetStoreDbContext db;

        public ProductService(PetStoreDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public void AddProduct(AddProductInputServiceModel model)
        {
            try
            {
                Product product = this.mapper.Map<Product>(model);

                this.db.Products.Add(product);
                this.db.SaveChanges();


            }
            catch (Exception)
            {

                throw new ArgumentException(ExceptionMessages.InvalidProductType);
            }
        }

        public void EditProduct(string id, EditProductInputServiceModel model)
        {
            try
            {
                Product product = this.mapper.Map<Product>(model);

                Product productToUpdate = this.db
                    .Products
                    .Find(id);

                if(productToUpdate == null)
                {
                    throw new ArgumentException(ExceptionMessages.ProductNotFound);
                }

                productToUpdate.Name = product.Name;
                productToUpdate.ProductType = product.ProductType;
                productToUpdate.Price = product.Price;

                this.db.SaveChanges();

            }
            catch (ArgumentException ae)
            {

                throw ae;
            }
            catch (Exception)
            {
                throw new ArgumentException(ExceptionMessages.InvalidProductType);
            }
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
            Product productToRemove = this.db
                .Products
                .Find(id);

            if(productToRemove == null)
            {
                throw new ArgumentException(ExceptionMessages.ProductNotFound);
            }

            this.db.Products.Remove(productToRemove);
            int rowAffected = this.db.SaveChanges();

            bool wasDeleted = rowAffected == 1;

            return wasDeleted;
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
