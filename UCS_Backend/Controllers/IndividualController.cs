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
    [Route("api/{controller}")]

    public class IndividualController : ControllerBase
    {
        private IIndividualRepository _individualRepository;

        public IndividualController(IIndividualRepository individualRepository){
            this._individualRepository = individualRepository;
        }
        
        [HttpGet()]
        public IEnumerable<Individual> GetAllIndividuals()
        {
            var individuals = this._individualRepository.GetAllIndividuals();

            return individuals;
        }

    }
}

