using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using Service.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerService.webApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        //private readonly IRepository<Company> repository;

        private readonly Iservice<CompanyDto> service;
        private readonly ICompanyService companyService;

        public CompanyController(Iservice<CompanyDto>service, ICompanyService companyService)//IRepository<Company> repository,
        {
            //this.repository = repository;
            this.service = service;
            this.companyService = companyService;
        }
        // GET: api/<CompanyController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> Get()
        {
            var companies= await service.GetAllAsync() ;
            return Ok(companies);
        }

        // GET api/<CompanyController>/5
        [HttpGet("{id}")]
        public async Task<CompanyDto> Get(int id)
        {
            return await service.GetByIdAsync(id);
        }

        // POST api/<CompanyController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CompanyDto company)
        {
           var newCompany= await service.AddAsync(company);
            return Ok(newCompany);
        }

        // PUT api/<CompanyController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CompanyDto company)
        {
            var newCompany = await service.UpdateAsync(id, company);
            return Ok(newCompany);
        }

        // DELETE api/<CompanyController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await service.DeleteAsync(id);
            return NoContent();

        }

        [HttpGet("GetWeeklyImprovement/{id}")]
        public async Task<IActionResult> GetWeeklyImprovement(int id)
        {
            var scores = await companyService.GetWeeklyImprovementAsync(id);
            return Ok(scores);
        }
    }
}
