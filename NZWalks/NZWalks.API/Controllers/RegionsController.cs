using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegionsController : Controller
    {
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRegionsAsync()
        {
            var regions = await regionRepository.GetAllAsync();

            // return DTO regions, not domain model
            //var regionsDTO = new List<Models.DTO.Region>();
            //regions.ToList().ForEach(region => 
            //{ 
            //    var regionDTO = new Models.DTO.Region()
            //    {
            //        Id= region.Id,
            //        Code= region.Code,
            //        Name= region.Name,
            //        Area= region.Area,
            //        Lat= region.Lat,
            //        Long= region.Long,
            //        Population= region.Population
            //    };
            //    regionsDTO.Add(regionDTO);
            //});

            var regionsDTO = mapper.Map<List<Models.DTO.Region>>(regions);

            return Ok(regionsDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetRegionAsync")]
        public async Task<IActionResult> GetRegionAsync([FromRoute] Guid id)
        {
            var region = await regionRepository.GetAsync(id);

            if(region == null)
            {
                return NotFound();
            }

            var regionDTO = mapper.Map<Models.DTO.Region>(region);
            return Ok(regionDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddRegionAsync(AddRegionRequest addRegionRequest)
        {
            // Request(DTO) to domain model
            var region = new Models.Domain.Region()
            {
                Code = addRegionRequest.Code,
                Area = addRegionRequest.Area,
                Lat = addRegionRequest.Lat,
                Long = addRegionRequest.Long,
                Name = addRegionRequest.Name,
                Population = addRegionRequest.Population
            };
            // Pass details to repository
            region = await regionRepository.AddAsync(region);
            // Convert back to DTO
            var regionDTO = mapper.Map<Models.DTO.Region>(region);

            return CreatedAtAction(nameof(GetRegionAsync), new {id = regionDTO.Id},regionDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteRegionAsync([FromRoute] Guid id)
        {
            // Get region from database
            var region = await regionRepository.DeleteAsync(id);
            // If null notfound
            if (region == null)
            {
                return NotFound();
            }

            // Convert response back to DTO
            var regionDTO = mapper.Map<Models.DTO.Region>(region);

            // Return Ok response
            return Ok(regionDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateRegionAsync([FromRoute] Guid id, [FromBody] UpdateRegionRequest updateRegionRequest)
        {
            // Request(DTO) to domain model
            var region = await GetRegionAsync(id);
            if (region != null)
            {
                //Update Details
                var updatedRegion = new Models.Domain.Region()
                {
                    Code = updateRegionRequest.Code,
                    Area = updateRegionRequest.Area,
                    Lat = updateRegionRequest.Lat,
                    Long = updateRegionRequest.Long,
                    Name = updateRegionRequest.Name,
                    Population = updateRegionRequest.Population
                };

                updatedRegion = await regionRepository.UpdateAsync(id, updatedRegion);

                if (updatedRegion != null)
                {
                    var regionDTO = mapper.Map<Models.DTO.Region>(updatedRegion);
                    return Ok(regionDTO);
                }
            }

            return NotFound();
        }
    }
}
