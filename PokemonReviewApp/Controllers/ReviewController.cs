using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IReviewRepository _repository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;

        public ReviewController(
            IMapper mapper, 
            IReviewRepository reviewRepository,
            IPokemonRepository pokemonRepository,
            IReviewerRepository reviewerRepository
            )
        {
            _mapper = mapper;
            _repository = reviewRepository;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
        }
        
        [HttpGet]
        public IActionResult GetReviews() 
        {
            var result = _mapper.Map<List<ReviewDto>>(_repository.GetReviews());

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(result);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            var result = _mapper.Map<ReviewDto>(_repository.GetReview(reviewId));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(result);
        }

        [HttpGet("pokemon/{pokeId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsForAPokemon(int pokeId)
        {
            var result = _mapper.Map<List<ReviewDto>>(_repository.GetReviewsOfAPokemon(pokeId));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId, [FromBody] ReviewDto reviewCreate)
        {
            if (reviewCreate == null) return BadRequest(ModelState);

            var review = _repository.GetReviews()
                .Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (review != null)
            {
                ModelState.AddModelError("", "Review Already Exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var reviewMap = _mapper.Map<Review>(reviewCreate);

            reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);
            reviewMap.Reviewer = _reviewerRepository.GetReviewer(pokeId);


            if (!_repository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Created.");
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updatedReview)
        {
            if (updatedReview == null) return BadRequest(ModelState);

            if (reviewId != updatedReview.Id) return BadRequest(ModelState);

            if (!_repository.ReviewExists(reviewId)) return NotFound();

            if (!ModelState.IsValid) return BadRequest();

            var reviewMap = _mapper.Map<Review>(updatedReview);

            if (!_repository.UpdateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something Went Wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Review Updated Successfully");

        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_repository.ReviewExists(reviewId)) return NotFound();

            var review = _repository.GetReview(reviewId);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_repository.DeleteReview(review))
            {
                ModelState.AddModelError("", "Something Went Wrong");
            }

            return Ok("Deletion Complete");
        }
    }
}
