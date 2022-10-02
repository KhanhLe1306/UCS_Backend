using System;
using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UCS_Backend.Repositories
{
    public class IndividualRepository : IIndividualRepository, IBaseRepository<Individual>
    {
        private DataContext dataContext;

        public IndividualRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public IEnumerable<Individual> GetAll => this.dataContext.Individuals.AsEnumerable();

        public async Task<List<Individual>> GetAllIndividuals()
        {
            return await this.dataContext.Individuals.ToListAsync();
        }

        public Individual? GetIndividualById(int id)
        {
            var res = from i in this.dataContext.Individuals
                      where i.IndividualId == id
                      select new Individual
                      {
                          IndividualId = i.IndividualId,
                          FirstName = i.FirstName,
                          LastName = i.LastName,
                      };
            
            return res.FirstOrDefault();
        }
        public Individual AddIndividual(Individual individual)
        {
            var res = dataContext.Individuals.Add(individual).Entity;
            dataContext.SaveChanges();
            return res;
        }

        public Individual Add(Individual individual)
        {
            var res = dataContext.Individuals.Add(individual).Entity;
            dataContext.SaveChanges();
            return res;
        }

        public void Update(Individual individual)
        {
            var temp = this.dataContext.Individuals.Find(individual.IndividualId);
            if(temp != null)
            {
                temp.FirstName = individual.FirstName;
                temp.LastName = individual.LastName;
                dataContext.SaveChanges();
            }
        }

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

        public void Delete(Individual individual)
        {
            this.dataContext.Individuals.Remove(individual);
            this.dataContext.SaveChanges();
        }
    }
}

