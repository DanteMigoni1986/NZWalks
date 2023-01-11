using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;

        public RegionRepository(NZWalksDbContext nZWalksDbContext)
        {
            this.nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<Region> AddAsync(Region region)
        {
            region.Id = Guid.NewGuid();
            await nZWalksDbContext.AddAsync(region);
            await nZWalksDbContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region> DeleteAsync(Guid id)
        {
            var region = await GetAsync(id);
            if (region != null)
            {
                nZWalksDbContext.Regions.Remove(region);
                await nZWalksDbContext.SaveChangesAsync();
                return region;
            }

            return null;
        }

        public async Task<bool> Exists(Guid id)
        {
            return await nZWalksDbContext.Regions.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            return await nZWalksDbContext.Regions.ToListAsync();
        }

        public async Task<Region> GetAsync(Guid id)
        {
            return await nZWalksDbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region> UpdateAsync(Guid id, Region region)
        {
            var existingRegion = await GetAsync(id);
            if(existingRegion != null)
            {
                existingRegion.Name = region.Name;
                existingRegion.Code = region.Code;
                existingRegion.Area= region.Area;
                existingRegion.Lat= region.Lat;
                existingRegion.Long= region.Long;
                existingRegion.Population= region.Population;
                await nZWalksDbContext.SaveChangesAsync();
                return existingRegion;
            }
            return null;
        }
    }
}
