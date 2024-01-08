using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>,ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext):base(repositoryContext)
        {

        }


        public async Task <PagedList<Company>> GetCompaniesAsync(CompanyParameters companyParameters, bool trackChanges)
        {

            var companies= await FindAll(trackChanges)
                .Search(companyParameters.searchTerm)
                .Sort(companyParameters.OrderBy)
                .ToListAsync();
            return PagedList<Company>.ToPagedList(companies,companyParameters.PageNumber,companyParameters.PageSize);
        }

        public async Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges)
        {
            return await FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefaultAsync(); 
        }
        public void CreateCompany(Company company)
        {
            Create(company);
        }

        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            return await FindByCondition(x=> ids.Contains(x.Id),trackChanges).ToListAsync();
        }

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }
    }
}
