using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UCS_Backend.Controllers
{
    [ApiController]
    [Route("api/{controller}/")]

    public class IndividualController : ControllerBase
    {
        private IIndividualRepository _individualRepository;

        public IndividualController(IIndividualRepository individualRepository) {
            this._individualRepository = individualRepository;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Individual>>> GetAllIndividuals()
        {
            var individuals = await this._individualRepository.GetAllIndividuals();

            return individuals;
        }

        [HttpGet("{id}")]
        public Individual? GetIndividualById(int id)
        {
            return this._individualRepository.GetIndividualById(id);
        }

        [HttpPost()]
        public Individual AddIndividual(Individual individual)
        {
            return _individualRepository.AddIndividual(individual);
        }

        [HttpPut()]
        public void UpdateIndividual(Individual individual)
        {
            _individualRepository.Update(individual);
        }

        [HttpDelete()]
        public void DeleteIndividual(Individual individual)
        {
            _individualRepository.Delete(individual);
        }
    }
}

