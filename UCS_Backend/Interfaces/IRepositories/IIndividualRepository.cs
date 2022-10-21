using System;
using UCS_Backend.Models;

namespace UCS_Backend.Interfaces
{
    public interface IIndividualRepository : IBaseRepository<Individual>
    {
        Task<List<Individual>> GetAllIndividuals();
        Task<Individual?> GetIndividualById(int id);
        Task<Individual> AddIndividual(Individual individual);
        Task<(bool, Individual)> UpdateIndividual(Individual individual);
        Task<bool> DeleteIndividual(Individual individual);
    }
}

