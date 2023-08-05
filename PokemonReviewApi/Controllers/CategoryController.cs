using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers;
[Route("api/[Controller]")]
[ApiController]
public class CategoryController : ControllerBase
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
    public IActionResult GetCategories()
    {
        var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return Ok(categories);
    }
    
    [HttpGet("{categoryId}")]
    [ProducesResponseType(200, Type = typeof(Category))]
    [ProducesResponseType(400)]
    public IActionResult GetCategory(int categoryId)
    {
        if (!_categoryRepository.CategoryExist(categoryId))
            return NotFound();

        var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(category);
    }

    [HttpGet("Pokemon/{CategoryId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonByCategoryId(int categoryId)
    {
        var pokemons = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetPokemonByCategory(categoryId));

        if (!ModelState.IsValid)
            return BadRequest();

        return Ok(pokemons);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
    {
        if (categoryCreate == null)
            return BadRequest(ModelState);

        var category = _categoryRepository.GetCategories()
            .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
            .FirstOrDefault();

        if(category != null)
        {
            ModelState.AddModelError("", "Category already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var categoryMap = _mapper.Map<Category>(categoryCreate);

        if(!_categoryRepository.CreateCategory(categoryMap))
        {
            ModelState.AddModelError("", "Something went wrong while savin");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{countryId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategory)
    {
        if (updatedCategory == null)
            return BadRequest(ModelState);

        if (categoryId != updatedCategory.Id)
            return BadRequest(ModelState);

        if (!_categoryRepository.CategoryExist(categoryId))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest();

        var categoryMap = _mapper.Map<Category>(updatedCategory);

        if (!_categoryRepository.UpdateCategory(categoryMap))
        {
            ModelState.AddModelError("", "Something went wrong updating category");
            return StatusCode(500, ModelState);
        }

        return NoContent();

    }

    [HttpDelete("categoryId")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult DeleteCategory(int categoryId)
    {
        if (!_categoryRepository.CategoryExist(categoryId))
            return NotFound();

        var categoryToDelete = _categoryRepository.GetCategory(categoryId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_categoryRepository.DeleteCategory(categoryToDelete))
        {
            ModelState.AddModelError("", "Something went wrong deleting category");
            return StatusCode(500, ModelState);
        }

        return NoContent();

    }

}