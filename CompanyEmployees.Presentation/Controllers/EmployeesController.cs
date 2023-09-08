using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Filters;
using CompanyEmployees.ActionFilters;
using Shared.RequestFeatures;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public EmployeesController(IServiceManager serviceManager) => _service = serviceManager;
        [HttpGet]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
            [FromQuery]EmployeeParameters employeeParameters)
        {

            var employeesAndMetaData = await _service.EmployeeService.GetEmployeesForCompanyAsync(companyId,employeeParameters, trackChanges:
            false);
            Response.Headers.Add("X-Pagination",
JsonSerializer.Serialize(employeesAndMetaData.metaData));
            return Ok(employeesAndMetaData.employeeDtos);


        }
        [HttpGet("{employeeId:guid}",Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            var employee = await _service.EmployeeService.GetEmployeeAsync(companyId, employeeId, trackChanges: false);
            return Ok(employee);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeForCreationDto)
        {
            var employeeToReturn = await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employeeForCreationDto, trackChanges: false);
            return CreatedAtRoute("GetEmployeeForCompany", new
            {
                companyId,
                employeeId = employeeToReturn.Id
            }, employeeToReturn);
        }

        [HttpPost("collection")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeesForCompanyCollection(Guid companyId, 
            [FromBody] IEnumerable<EmployeeForCreationDto> employeeForCreationDtos)
        {
            await _service.EmployeeService.CreateEmployeesForCompanyCollectionAsync(companyId
                ,employeeForCreationDtos,trackChanges:false
                );
            return NoContent();

        }

        [HttpDelete("{employeeId:guid}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            await _service.EmployeeService.DeleteEmployeeAsync(companyId, employeeId, trackChanges: false);
            return NoContent();
        }

        [HttpPut("{employeeId:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] EmployeeForUpdateDto employee)
        {
            await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, employeeId, employee, false, true);
            return NoContent();
        }

        [HttpPatch("{employeeId:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest("PatchDoc is null");
            }

            var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, employeeId, false, true);
            patchDoc.ApplyTo(result.employeeToPatch, ModelState);
            TryValidateModel(result.employeeToPatch);
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            await _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity);
            return NoContent();
        }
    }
}
