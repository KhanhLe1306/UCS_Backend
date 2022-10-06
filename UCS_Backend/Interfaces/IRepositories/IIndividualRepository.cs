﻿using System;
using UCS_Backend.Models;

namespace UCS_Backend.Interfaces
{
    public interface IIndividualRepository : IBaseRepository<Individual>
    {
        Task<List<Individual>> GetAllIndividuals();
        Individual? GetIndividualById(int id);
        Individual AddIndividual(Individual individual);
    }
}

