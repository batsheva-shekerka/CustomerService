using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using Microsoft.AspNetCore.Authorization;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerService.webApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallController : ControllerBase
    {
        private readonly Iservice<CallDto> service;
        private readonly ICallService<CallDto> callService;

        public CallController(Iservice<CallDto> service, ICallService<CallDto> callService)
        {
            this.service = service;
            this.callService = callService;
        }

        // GET: api/<CallController>
        [HttpGet]
        [Authorize(Roles = "SystemManager")]
        public async Task<IEnumerable<CallDto>> Get()
        {
            return await service.GetAllAsync();
        }

        // GET api/<CallController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SystemManager")]
        public async Task<CallDto> Get(int id)
        {
            return await service.GetByIdAsync(id);
        }

        // POST api/<CallController>
        [HttpPost]
        [Authorize(Roles = "Admin,SystemManager")]
        public async void Post([FromBody] CallDto call)
        {
            await service.AddAsync(call);
        }

        // PUT api/<CallController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SystemManager")]
        public async Task Put(int id, [FromBody] CallDto call)
        {
            await service.UpdateAsync(id,call);
        }

        // DELETE api/<CallController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SystemManager")]
        public async Task Delete(int id)
        {
            await service.DeleteAsync(id);
        }


        [HttpGet("operator/{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]

        public async Task<IEnumerable<CallDto>> GetByIdOperator(int id)
        {
            return await callService.GetAllByOperatorAsync(id);
        }

        [HttpGet("GetDailyImprovement/{id}")]
        [Authorize(Roles = "Admin,SystemManager")]
        public async Task<ScoreCompanyDto> GetDailyImprovementAsync(int id)
        {
            return await callService.GetDailyImprovementAsync(id);
        }
    }
}
