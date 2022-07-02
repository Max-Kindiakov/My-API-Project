using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiVersion("2.0")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenApiSpecNP")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksVer2Controller : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksVer2Controller(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }
        /// <summary>
        /// Get list of all national parks.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetNationalParks()
        {
            var obj = _npRepo.GetNationalParks().FirstOrDefault();
            
            return Ok(_mapper.Map<NationalParkDto>(obj));
        }
        
    }
}

