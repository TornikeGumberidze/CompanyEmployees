﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace CompanyEmployees.Presentation.Controllers
{

    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public CompaniesController(IServiceManager serviceManager)=>_service = serviceManager;
        [HttpGet]
        public IActionResult GetCompanies()
        {
            try
            {
                return Ok(_service.CompanyService.GetAllCompanies(false));
            }
            catch {
                return StatusCode(500, "Internal server error");
            }
        }
    }

}