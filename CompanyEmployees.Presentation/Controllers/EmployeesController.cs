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

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public EmployeesController(IServiceManager serviceManager) => _service = serviceManager;
        [HttpGet]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        {
            var employees = await _service.EmployeeService.GetEmployeesAsync(companyId, trackChanges:
            false);
            return Ok(employees);
        }
        [HttpGet("{employeeId:guid}",Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            Console.WriteLine($"EmployeeId is {employeeId}");
            var employee = await _service.EmployeeService.GetEmployeeAsync(companyId, employeeId, trackChanges: false);
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeForCreationDto)
        {
            if (employeeForCreationDto == null)
            {
                return BadRequest("EmployeeForCreationDto object is null");
            }
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            var employeeToReturn = await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employeeForCreationDto, trackChanges: false);
            return CreatedAtRoute("GetEmployeeForCompany", new
            {
                companyId,
                employeeId = employeeToReturn.Id
            }, employeeToReturn);
        }

        [HttpDelete("{employeeId:guid}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            await _service.EmployeeService.DeleteEmployeeAsync(companyId, employeeId, trackChanges: false);
            return NoContent();
        }

        [HttpPut("{employeeId:guid}")]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] EmployeeForUpdateDto employee)
        {
            if (employee is null)
            {
                return BadRequest("EmployeeForUpdateDto object is null");
            }
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
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
