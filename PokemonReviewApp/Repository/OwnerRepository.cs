using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext _dataContext;
        public OwnerRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Owner GetOwner(int ownerId)
        {
            return _dataContext.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
        {
            return _dataContext.PokemonOwners.Where(o => o.Pokemon.Id == pokeId).Select(o => o.Owner).ToList();
        }

        public ICollection<Owner> GetOwners()
        {
            return _dataContext.Owners.OrderBy(o => o.Id).ToList();
        }

        public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
        {
            return _dataContext.PokemonOwners.Where(p => p.Pokemon.Id == ownerId).Select(p => p.Pokemon).ToList();
        }

        public bool OwnerExists(int ownerId)
        {
            return _dataContext.Owners.Any(c => c.Id == ownerId);
        }

        public bool CreateOwner(Owner owner)
        {
            _dataContext.Owners.Add(owner);
            return Save();
        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateOwner(Owner owner)
        {
            _dataContext.Update(owner);
            return Save();
        }

        public bool DeleteOwner(Owner owner)
        {
            _dataContext.Remove(owner);
            return Save();
        }
    }
}
