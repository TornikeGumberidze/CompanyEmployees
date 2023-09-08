using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Shared.RequestFeatures;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        Task<(IEnumerable<EmployeeDto> employeeDtos, MetaData metaData)> GetEmployeesForCompanyAsync(Guid companyId,EmployeeParameters employeeParameters, bool trackChanges);
        Task<EmployeeDto > GetEmployeeAsync(Guid companyId,Guid employeeId,bool trackChanges);
        Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employee,bool trackChanges);
        Task DeleteEmployeeAsync(Guid companyId, Guid employeeId,bool trackChanges);
        Task UpdateEmployeeForCompanyAsync
            (Guid companyId, Guid employeeId, EmployeeForUpdateDto employee,bool compTrackChanges,bool empTrackChanges);
        Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges);
        Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee
        employeeEntity);
        Task CreateEmployeesForCompanyCollectionAsync(Guid companyId,
             IEnumerable<EmployeeForCreationDto> employees,bool trackChanges);

    }
}
