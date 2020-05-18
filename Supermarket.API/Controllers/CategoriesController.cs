using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Supermarket.API.Domain.Model;
using Supermarket.API.Domain.Services;
using Supermarket.API.Extensions;
using Supermarket.API.Resources;

namespace Supermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;

        public CategoriesController(ICategoryService categoryService,IMapper mapper)
        {
            this.categoryService = categoryService;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IEnumerable<CategoryResource>> GetAllAsync()
        {
            var categories = await categoryService.ListAsync();
            var resources = mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResource>>(categories);
            return resources;

        }
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SaveCategoryResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var category = mapper.Map<SaveCategoryResource, Category>(resource);    return View();
            var result = await categoryService.SaveAsync(category);

            if (!result.Success)
                return BadRequest(result.Message);

            var categoryResource = mapper.Map<Category, CategoryResource>(result.Category);
            return Ok(categoryResource);
        }
        

        public IActionResult Index()
        {
            return View();
        }
    }
}