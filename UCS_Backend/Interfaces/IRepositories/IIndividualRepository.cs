using System;
using UCS_Backend.Models;

namespace UCS_Backend.Interfaces
{
    /// <summary>
    /// creates class for individual repo with task to get individuals
    /// </summary>
    public interface IIndividualRepository : IBaseRepository<Individual>
    {
        Task<List<Individual>> GetAllIndividuals();
        Task<Individual?> GetIndividualById(int id);
        Task<Individual> AddIndividual(Individual individual);
        Task<(bool, Individual)> UpdateIndividual(Individual individual);
        Task<bool> DeleteIndividual(Individual individual);
    }
}

