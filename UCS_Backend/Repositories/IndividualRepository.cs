using System;
using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UCS_Backend.Repositories
{

    /// <summary>
    /// Creates a class for IndividualRepositoty
    /// </summary> 
    public class IndividualRepository : IIndividualRepository, IBaseRepository<Individual>
    {
        private DataContext dataContext;
        /// <summary>
        /// create individualRepo
        /// </summary>
        /// <param name="dataContext"></param>

        public IndividualRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public IEnumerable<Individual> GetAll => this.dataContext.Individuals.AsEnumerable();
/// <summary>
/// creates task to get all individuals
/// </summary>
/// <returns></returns>
        public async Task<List<Individual>> GetAllIndividuals()
        {
            return await this.dataContext.Individuals.ToListAsync();
        }
/// <summary>
/// updates task to get individualbyId
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
        public async Task<Individual?> GetIndividualById(int id)
        {
            var res = from i in this.dataContext.Individuals
                      where i.IndividualId == id
                      select new Individual
                      {
                          IndividualId = i.IndividualId,
                          FirstName = i.FirstName,
                          LastName = i.LastName,
                      };
            
            return await res.FirstAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public async Task<Individual> AddIndividual(Individual individual)
        {
            var res = (await dataContext.Individuals.AddAsync(individual)).Entity;
            await dataContext.SaveChangesAsync();
            return res;
        }
/// <summary>
/// add individual to data context
/// </summary>
/// <param name="individual"></param>
/// <returns></returns>
        public Individual Add(Individual individual)
        {
            var res = dataContext.Individuals.Add(individual).Entity;
            dataContext.SaveChanges();
            return res;
        }
/// <summary>
/// Updates individual by first name and lastname
/// </summary>
/// <param name="individual"></param>
/// <returns></returns>
        public async Task<(bool, Individual)> UpdateIndividual(Individual individual)
        {
            var temp = await this.dataContext.Individuals.Where(i => i.IndividualId == individual.IndividualId).FirstAsync(); 
            if(temp != null)
            {
                temp.FirstName = individual.FirstName;
                temp.LastName = individual.LastName;
                await dataContext.SaveChangesAsync();
                return (true, temp);
            }
            else
            {
                return (false, individual);
            }
        }
/// <summary>
/// Remove individuals 
/// </summary>
/// <param name="individual"></param>
/// <returns></returns>
        public async Task<bool> DeleteIndividual(Individual individual)
        {
            var res = this.dataContext.Individuals.Remove(individual).Entity;
            if (res != null)
            {
                return true;
            }else
            {
                return false;
            }
        }
/// <summary>
/// find individual by id
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
        public Individual? FindById(int id)
        {
            /*var res = from i in this.dataContext.Individuals
                      where i.IndividualId == id
                      select new Individual
                      {
                          IndividualId = i.IndividualId,
                          FirstName = i.FirstName,
                          LastName = i.LastName,
                      };
            return res.FirstOrDefault();*/

            return dataContext.Individuals.Find(id);
        }
/// <summary>
/// method to delete individual
/// </summary>
/// <param name="individual"></param>
        public void Delete(Individual individual)
        {
            this.dataContext.Individuals.Remove(individual);
            this.dataContext.SaveChanges();
        }
/// <summary>
/// updates individual
/// </summary>
/// <param name="idnividual"></param>
        public void Update(Individual idnividual)
        {
            
        }


    }
}

