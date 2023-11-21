using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategories()
        {
            var cats = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(cats);
        }

        [HttpGet("{catId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int catId)
        {
            var cat = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(catId));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(cat);
        }

        [HttpGet("pokemon/{catId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonFromCatId(int catId)
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonByCategory(catId));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(pokemons);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            if (categoryCreate == null) return BadRequest(ModelState);

            var category = _categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", "Category Already Exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categoryMap = _mapper.Map<Category>(categoryCreate);

            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Created.");
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategory)
        {
            if (updatedCategory == null) return BadRequest(ModelState);

            if (categoryId != updatedCategory.Id) return BadRequest(ModelState);

            if (!_categoryRepository.CategoryExists(categoryId)) return NotFound();

            if (!ModelState.IsValid) return BadRequest();

            var catMap = _mapper.Map<Category>(updatedCategory);

            if (!_categoryRepository.UpdateCategory(catMap))
            {
                ModelState.AddModelError("", "Something Went Wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Category Updated Successfully");

        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId)) return NotFound();

            var category = _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("", "Something Went Wrong");
            }

            return Ok("Deletion Complete");
        }

    }
}
