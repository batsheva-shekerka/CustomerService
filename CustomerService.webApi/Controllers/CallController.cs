using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;

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
        public async Task<IEnumerable<CallDto>> Get()
        {
            return await service.GetAllAsync();
        }

        // GET api/<CallController>/5
        [HttpGet("{id}")]
        public async Task<CallDto> Get(int id)
        {
            return await service.GetByIdAsync(id);
        }

        // POST api/<CallController>
        [HttpPost]
        public async void Post([FromBody] CallDto call)
        {
            await service.AddAsync(call);
        }

        // PUT api/<CallController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] CallDto call)
        {
            await service.UpdateAsync(id,call);
        }

        // DELETE api/<CallController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await service.DeleteAsync(id);
        }


        [HttpGet("operator/{id}")]
        public async Task<IEnumerable<CallDto>> GetByIdOperator(int id)
        {
            return await callService.GetAllByOperatorAsync(id);
        }

        [HttpGet("GetDailyImprovement/{id}")]
        public async Task<ScoreCompanyDto> GetDailyImprovementAsync(int id)
        {
            return await callService.GetDailyImprovementAsync(id);
        }
    }
}
