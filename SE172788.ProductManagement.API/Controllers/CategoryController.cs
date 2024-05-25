using Microsoft.AspNetCore.Mvc;
using SE172788.ProductManagement.API.Model.CategoryModel;
using SE172788.ProductManagement.Repo.Entities;
using SE172788.ProductManagement.Repo.Repositories;
using System;

namespace SE172788.ProductManagement.API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public CategoryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                Console.WriteLine("Attempting to retrieve all categories...");

                var responseCategories = _unitOfWork.CategoryRepository.Get();

                if (responseCategories == null)
                {
                    Console.WriteLine("No categories found.");
                    return NotFound("No categories found.");
                }

                Console.WriteLine("Categories retrieved successfully.");
                return Ok(responseCategories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving categories: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                Console.WriteLine($"Attempting to retrieve category with ID {id}...");

                var responseCategory = _unitOfWork.CategoryRepository.GetByID(id);

                if (responseCategory == null)
                {
                    Console.WriteLine($"Category with ID {id} not found.");
                    return NotFound($"Category with ID {id} not found.");
                }

                Console.WriteLine($"Category with ID {id} retrieved successfully.");
                return Ok(responseCategory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving category with ID {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel requestCategoryModel)
        {
            try
            {
                if (requestCategoryModel == null)
                {
                    Console.WriteLine("Invalid category model received.");
                    return BadRequest("Invalid category model.");
                }

                Console.WriteLine("Attempting to create new category...");

                var categoryEntity = new Category
                {
                    CategoryName = requestCategoryModel.CategoryName
                };

                _unitOfWork.CategoryRepository.Insert(categoryEntity);
                _unitOfWork.Save();

                Console.WriteLine("Category created successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating category: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, CategoryModel requestCategoryModel)
        {
            try
            {
                if (requestCategoryModel == null)
                {
                    Console.WriteLine("Invalid category model received.");
                    return BadRequest("Invalid category model.");
                }

                Console.WriteLine($"Attempting to update category with ID {id}...");

                var existedCategoryEntity = _unitOfWork.CategoryRepository.GetByID(id);
                if (existedCategoryEntity == null)
                {
                    Console.WriteLine($"Category with ID {id} not found.");
                    return NotFound($"Category with ID {id} not found.");
                }

                existedCategoryEntity.CategoryName = requestCategoryModel.CategoryName;

                _unitOfWork.CategoryRepository.Update(existedCategoryEntity);
                _unitOfWork.Save();

                Console.WriteLine($"Category with ID {id} updated successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating category with ID {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                Console.WriteLine($"Attempting to delete category with ID {id}...");

                var existedCategoryEntity = _unitOfWork.CategoryRepository.GetByID(id);
                if (existedCategoryEntity == null)
                {
                    Console.WriteLine($"Category with ID {id} not found.");
                    return NotFound($"Category with ID {id} not found.");
                }

                _unitOfWork.CategoryRepository.Delete(existedCategoryEntity);
                _unitOfWork.Save();

                Console.WriteLine($"Category with ID {id} deleted successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting category with ID {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
