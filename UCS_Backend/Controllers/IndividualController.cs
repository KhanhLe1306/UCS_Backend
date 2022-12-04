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
        /// <summary>
        /// Creates a class for IndividualController
        /// </summary> 
    public class IndividualController : ControllerBase
    {
        private IIndividualRepository _individualRepository;
        /// <summary>
        /// individual repo created
        /// </summary>
        /// <param name="individualRepository"></param>
        public IndividualController(IIndividualRepository individualRepository) {
            this._individualRepository = individualRepository;
        }
        /// <summary>
        /// get all individuals
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Individual>>> GetAllIndividuals()
        {
            var individuals = await this._individualRepository.GetAllIndividuals();

            return individuals;
        }
        /// <summary>
        /// get individuals by individual_id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Individual?>> GetIndividualById(int id)
        {
            return await this._individualRepository.GetIndividualById(id);
        }
        /// <summary>
       /// add individuals
       /// </summary>
       /// <param name="individual"></param>
       /// <returns></returns>
        [HttpPost()]
        public async Task<ActionResult<Individual>> AddIndividual(Individual individual)
        {
            return await _individualRepository.AddIndividual(individual);
        }
       /// <summary>
       /// updates individual
       /// </summary>
       /// <param name="individual"></param>
       /// <returns></returns>
        [HttpPut()]
        public async Task<ActionResult<(bool, Individual)>> UpdateIndividual(Individual individual)
        {
            return await _individualRepository.UpdateIndividual(individual);
        }
        /// <summary>
        /// deletes individual
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        [HttpDelete()]
        public async Task<ActionResult<bool>> DeleteIndividual(Individual individual)
        {
            return await _individualRepository.DeleteIndividual(individual);
        }
    }
}

