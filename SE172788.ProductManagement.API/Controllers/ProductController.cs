using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE172788.ProductManagement.API.Model.ProductModel;
using SE172788.ProductManagement.Repo.Entities;
using SE172788.ProductManagement.Repo.Repositories;
using System.Linq.Expressions;

namespace SE172788.ProductManagement.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public ProductController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// SortBy (ProductId = 1, ProductName = 2, CategoryId = 3, UnitsInStock = 4, UnitPrice = 5)
        /// 
        /// SortType (Ascending = 1, Descending = 2)
        /// </summary>
        /// <param name="requestSearchProductModel"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public IActionResult SearchProduct([FromQuery] SearchProductModel requestSearchProductModel)
        {
            var sortBy = requestSearchProductModel.SortContent?.sortProductBy.ToString();
            var sortType = requestSearchProductModel.SortContent?.sortProductType?.ToString();

            Expression<Func<Product, bool>> filter = x =>
                (string.IsNullOrEmpty(requestSearchProductModel.ProductName) || x.ProductName.Contains(requestSearchProductModel.ProductName)) &&
                (!requestSearchProductModel.CategoryId.HasValue || x.CategoryId == requestSearchProductModel.CategoryId) &&
                (x.UnitPrice >= requestSearchProductModel.FromUnitPrice &&
                 (!requestSearchProductModel.ToUnitPrice.HasValue || x.UnitPrice <= requestSearchProductModel.ToUnitPrice.Value));

            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = query =>
            {
                if (!string.IsNullOrEmpty(sortBy))
                {
                    if (sortType == "1") // Ascending
                    {
                        return query.OrderBy(p => EF.Property<object>(p, sortBy));
                    }
                    else if (sortType == "2") // Descending
                    {
                        return query.OrderByDescending(p => EF.Property<object>(p, sortBy));
                    }
                }
                return query.OrderBy(p => p.ProductId); // Default sorting
            };

            var responseProducts = _unitOfWork.ProductRepository.Get(
                filter,
                orderBy,
                includeProperties: "",
                pageIndex: requestSearchProductModel.pageIndex,
                pageSize: requestSearchProductModel.pageSize
            );

            return Ok(responseProducts);
        }

        [HttpPost]
        public IActionResult CreateProduct(CreateProductModel requestCreateProductModel)
        {
            var productEntity = new Product
            {
                CategoryId = requestCreateProductModel.CategoryId,
                ProductName = requestCreateProductModel.ProductName,
                UnitPrice = requestCreateProductModel.UnitPrice,
                UnitsInStock = requestCreateProductModel.UnitsInStock,
            };
            _unitOfWork.ProductRepository.Insert(productEntity);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, CreateProductModel requestCreateProductModel)
        {
            var existedProductEntity = _unitOfWork.ProductRepository.GetByID(id);
            if (existedProductEntity != null)
            {
                existedProductEntity.CategoryId = requestCreateProductModel.CategoryId;
                existedProductEntity.ProductName = requestCreateProductModel.ProductName;
                existedProductEntity.UnitPrice = requestCreateProductModel.UnitPrice;
                existedProductEntity.UnitsInStock = requestCreateProductModel.UnitsInStock;

                _unitOfWork.ProductRepository.Update(existedProductEntity);
                _unitOfWork.Save();
                return Ok();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var existedProductEntity = _unitOfWork.ProductRepository.GetByID(id);
            if (existedProductEntity != null)
            {
                _unitOfWork.ProductRepository.Delete(existedProductEntity);
                _unitOfWork.Save();
                return Ok();
            }
            return NotFound();
        }
    }
}
