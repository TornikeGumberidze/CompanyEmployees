using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using CompanyEmployees.Presentation.ModelBinders;
using CompanyEmployees.ActionFilters;
using Shared.RequestFeatures;
using System.Text.Json;
using Marvin.Cache.Headers;

namespace CompanyEmployees.Presentation.Controllers
{

    [Route("api/companies")]
    [ApiController]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    public class CompaniesController : ControllerBase
    {


        private readonly IServiceManager _service;
        public CompaniesController(IServiceManager serviceManager) => _service = serviceManager;
        [HttpGet]
        public async Task< IActionResult > GetCompanies([FromQuery]CompanyParameters companyParameters)
        {
            var companies = await _service.CompanyService.GetCompaniesAsync(companyParameters,trackChanges: false);
            var companiesAndMetaData = await _service.CompanyService.GetCompaniesAsync( companyParameters, trackChanges:
            false);
            Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(companiesAndMetaData.MetaData));
            return Ok(companiesAndMetaData.companyDtos);
        }
        [HttpGet("{id:guid}", Name = "CompanyById")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task< IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);

            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id },
            createdCompany);
        }

        
        [HttpGet("collection", Name = "CompanyCollection")]//get by ids array but from swagger
        public async Task< IActionResult>GetCompanyCollectionFromSwagger([FromQuery] IEnumerable<Guid> ids)
        {
            var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
            return Ok(companies);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody]
IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result =
            await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
            return CreatedAtRoute("CompanyCollection",//naxe;
            result.companies);
        }
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id) {
            await _service.CompanyService.DeleteCompanyAsync(id,trackChanges: false);
            return NoContent();
        }
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))] 
        public async Task<IActionResult> UpdateCompany(Guid id,[FromBody]CompanyForUpdateDto company)
        {
            await _service.CompanyService.UpdateCompanyAsync(id,company,true);
            return NoContent(); 
        }
    }

}
