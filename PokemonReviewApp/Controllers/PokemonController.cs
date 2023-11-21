using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    // Attributes
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {

        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;
        private readonly IReviewRepository _reviewRepository;
        private readonly IOwnerRepository _ownerRepository;
        
        // ctor == constuctor
        public PokemonController(IPokemonRepository pokemonRepository,
            IMapper mapper,
            IOwnerRepository ownerRepository,
            IReviewRepository reviewRepository
            )
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
            _ownerRepository = ownerRepository;
            _reviewRepository = reviewRepository;

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons() 
        {
            // A way of automatically mapping the pokemon class to the pokemon DTO class.
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

            // Validation
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId) 
        {
            if(!_pokemonRepository.PokemonExists(pokeId)) 
            {
                return NotFound();
            }
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var pokemonRating = _pokemonRepository.GetPokemonRating(pokeId);

            if(!ModelState.IsValid)
                    return BadRequest(ModelState);

            return Ok(pokemonRating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
        {
            if (pokemonCreate == null) return BadRequest(ModelState);

            var pokemon = _pokemonRepository.GetPokemons()
                .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (pokemon != null)
            {
                ModelState.AddModelError("", "Pokemon Already Exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

            if (!_pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Created.");
        }

        [HttpPut("{pokemonId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(
            int pokemonId,
            [FromQuery] int ownerId,
            [FromQuery] int catId,
            [FromBody] PokemonDto updatedPokemon
            )
        {
            if (updatedPokemon == null) return BadRequest(ModelState);

            if (pokemonId != updatedPokemon.Id) return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExists(pokemonId)) return NotFound();

            if (!ModelState.IsValid) return BadRequest();

            var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);

            if (!_pokemonRepository.UpdatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something Went Wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Pokemon Updated Successfully");

        }

        [HttpDelete("{pokemonId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokemonId)
        {
            if (!_pokemonRepository.PokemonExists(pokemonId)) return NotFound();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var reviews = _reviewRepository.GetReviewsOfAPokemon(pokemonId);

            if(!_reviewRepository.DeleteReviews(reviews.ToList()))
            {
                ModelState.AddModelError("", "Somehthing went wrong when deleting reviews.");
            }

            var pokemon = _pokemonRepository.GetPokemon(pokemonId);


            if (!_pokemonRepository.DeletePokemon(pokemon))
            {
                ModelState.AddModelError("", "Something Went Wrong");
            }

            return Ok("Deletion Complete");
        }
    }
}
