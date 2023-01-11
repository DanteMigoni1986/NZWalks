using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;

        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync()
        {
            var walksDomain = await walkRepository.GetAllAsync();
            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walksDomain);
            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync([FromRoute] Guid id)
        {
            var walkDomain = await walkRepository.GetAsync(id);

            if(walkDomain == null)
            {
                return NotFound();
            }
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync([FromRoute] Guid id)
        {
            var walkDomain = await walkRepository.DeleteAsync(id);

            if (walkDomain == null)
            {
                return NotFound();
            }
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);
            return Ok(walkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync([FromBody] AddWalkRequest walkRequest)
        {
            var walkDomain = mapper.Map<Models.Domain.Walk>(walkRequest);
            walkDomain = await walkRepository.AddAsync(walkDomain);
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);
            return CreatedAtAction(nameof(GetWalkAsync), new {id = walkDTO.Id},walkDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id, [FromBody] UpdateWalkRequest walkRequest)
        {
            var walkDomain = await walkRepository.GetAsync(id);
            if (walkDomain != null)
            {
                //Update Details
                var updatedWalk = mapper.Map<Models.Domain.Walk>(walkRequest);

                updatedWalk = await walkRepository.UpdateAsync(id, updatedWalk);

                if (updatedWalk != null)
                {
                    var walkDTO = mapper.Map<Models.DTO.Walk>(updatedWalk);
                    return Ok(walkDTO);
                }
            }

            return NotFound();
        }
    }
}
