using System;
using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;

namespace UCS_Backend.Repositories
{
    public class IndividualRepository : IIndividualRepository
    {
        private DataContext dataContext;

        public IndividualRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<Individual> GetAllIndividuals()
        {
            return this.dataContext.Individuals.ToList();
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
    }
}

