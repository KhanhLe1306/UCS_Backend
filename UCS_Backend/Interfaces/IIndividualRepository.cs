using System;
using UCS_Backend.Models;

namespace UCS_Backend.Interfaces
{
    public interface IIndividualRepository
    {
        List<Individual> GetAllIndividuals();
        Individual? GetIndividualById(int id);
    }
}

