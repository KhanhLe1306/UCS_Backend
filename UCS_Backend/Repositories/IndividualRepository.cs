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
    }
}

