using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        public IRepositoryManager _repository;
        public ILoggerManager _logger;
        public IMapper _mapper;
        public EmployeeService(IRepositoryManager repository, ILoggerManager logger,IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employee, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var employeeEntity = _mapper.Map<Employee>(employee);
            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeToReturn;
        }


        public async Task DeleteEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            var employee = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges);
            if (employee == null)
            {
                throw new EmployeeNotFoundException(employeeId);
            }
            _repository.Employee.DeleteEmployee(employee);
            await _repository.SaveAsync();
        }


        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var employee = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges);
            if (employee == null)
            {
                throw new EmployeeNotFoundException(employeeId);
            }
            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return employeeDto;
        }


        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges);
            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges);
            if (employee == null)
            {
                throw new EmployeeNotFoundException(id);
            }
            var employeeToPatchToReturn = _mapper.Map<EmployeeForUpdateDto>(employee);
            return (employeeToPatchToReturn, employee);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            }

            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            return employeesDto;
        }

        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId, EmployeeForUpdateDto employee, bool compTrackChanges, bool empTrackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges);
            if (company == null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            var emp =await  _repository.Employee.GetEmployeeAsync(companyId, employeeId, empTrackChanges);
            if (emp == null)
            {
                throw new EmployeeNotFoundException(employeeId);
            }
            _mapper.Map(employee, emp);
            await _repository.SaveAsync();
        }
    }
}
