using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
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
            await CheckIfCompanyExists(companyId, trackChanges);
            var employeeEntity = _mapper.Map<Employee>(employee);
            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeToReturn;
        }

        public async Task CreateEmployeesForCompanyCollectionAsync(Guid companyId, 
            IEnumerable<EmployeeForCreationDto> employees, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);
            var employeeEntities = _mapper.Map<IEnumerable<Employee>>(employees);
            foreach(var employee in employeeEntities)
            {
                _repository.Employee.CreateEmployeeForCompany(companyId, employee);
            }
            await _repository.SaveAsync();
        }

        public async Task DeleteEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);
            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId,employeeId,trackChanges);
            _repository.Employee.DeleteEmployee(employee);
            await _repository.SaveAsync();
        }


        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId,employeeId,trackChanges);
            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return employeeDto;
        }


        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);
            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId,id,empTrackChanges);
            var employeeToPatchToReturn = _mapper.Map<EmployeeForUpdateDto>(employee);
            return (employeeToPatchToReturn, employee);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId,EmployeeParameters employeeParameters, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId,trackChanges);

            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId,employeeParameters, trackChanges);
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
            await CheckIfCompanyExists(companyId, compTrackChanges);
            var emp =await  GetEmployeeForCompanyAndCheckIfItExists(companyId,employeeId,empTrackChanges);
            _mapper.Map(employee, emp);
            await _repository.SaveAsync();
        }

        private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId,
            trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }
        private async Task <Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId,Guid id, bool trackChanges)
        {
            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if(employee is null)
            {
                throw new EmployeeNotFoundException(id);
            }
            return employee;
        }
    }
}
