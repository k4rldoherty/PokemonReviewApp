﻿using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemon(int id);
        Pokemon GetPokemon(string name);
        decimal GetPokemonRating(int pokeId);
        bool PokemonExists(int pokeId);
        bool CreatePokemon(int ownerId, int catId, Pokemon pokemon);
        bool Save();
        bool UpdatePokemon(int ownerId, int catId, Pokemon pokemon);
        bool DeletePokemon(Pokemon pokemon);
    }
}
