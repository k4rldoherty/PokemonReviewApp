using Microsoft.EntityFrameworkCore.Diagnostics;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;
        public PokemonRepository(DataContext context) 
        {
            _context = context;
        }

        public Pokemon GetPokemon(int id)
        {
            // Where clause --> SQL where clause, basically a filter function.
            // Search Id column for the matching Id and return the first 
            return _context.Pokemon.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemon(string name)
        {
            // Same thing, just name instead of ID
            return _context.Pokemon.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokeId)
        {
            var pokemonReviews = _context.Reviews.Where(p => p.Pokemon.Id == pokeId);
            if(pokemonReviews.Count() < 1) 
            {
                return 0;
            }

            // Get an average of the ratings and return a decimal value of the average.
            return ((decimal)pokemonReviews.Sum(r => r.Rating) / pokemonReviews.Count());
        }

        public ICollection<Pokemon> GetPokemons() 
        {
            return _context.Pokemon.OrderBy(p => p.Id).ToList();
        }

        public bool PokemonExists(int pokeId)
        {
            // Return a boolean value if a pokemon with given Id exists.
            return _context.Pokemon.Any(p => p.Id == pokeId);
        }

        public bool CreatePokemon(int ownerId, int catId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.Where(a => a.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(a => a.Id == catId).FirstOrDefault();
            
            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon,
            };

            _context.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon
            };

            _context.Add(pokemonCategory);

            _context.Add(pokemon);

            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePokemon(int ownerId, int catId, Pokemon pokemon)
        {
            _context.Update(pokemon);
            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon);
            return Save();
        }
    }
}
