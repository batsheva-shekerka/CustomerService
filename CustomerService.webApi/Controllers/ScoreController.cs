using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerService.webApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly IRepository<Score> repository;

        public ScoreController(IRepository<Score> repository)
        {
            this.repository = repository;
        }
        // GET: api/<ScoreController>
        [HttpGet]
        [Authorize(Roles = "SystemManager")]

        public async Task<IEnumerable<Score>> Get()
        {
            return await repository.GetAllAsync();
        }

        // GET api/<ScoreController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]
        public async Task<Score> Get(int id)
        {
            return await repository.GetByIdAsync(id);
        }

        // POST api/<ScoreController>
        [HttpPost]
        [Authorize(Roles = "Admin,SystemManager,Operator")]
        public async Task Post([FromBody] Score score)
        {
            await repository.AddAsync(score);
        }

        // PUT api/<ScoreController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SystemManager")]

        public async Task Put(int id, [FromBody] Score score)
        {
            await repository.UpdateAsync(id,score);
        }

        // DELETE api/<ScoreController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SystemManager")]
        public async Task Delete(int id)
        {
            await repository.DeleteAsync(id);
        }
    }
}
