using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;
using Common.Dto;
using Service.Interfaces;
using Service.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Drawing;
using CustomerService.webApi.Exceptions;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerService.webApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatorController : ControllerBase
    {
        private readonly Iservice<OperatorDto> service;
        private readonly IOperatorService operatorService;
        private readonly OperatorService _operatorService;


        public OperatorController(Iservice<OperatorDto> service, OperatorService operatorService,IOperatorService operatorService1)
        {
            this.service = service;
            this._operatorService = operatorService;
            this.operatorService = operatorService1;
        }
        // GET: api/<OperatorController>
        [HttpGet]
        [Authorize(Roles = "Admin,SystemManager")]
        public async Task<IActionResult> Get()
        {
            // 1.from-Token (Claims)
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"DEBUG: User Role is: {userRole}",Color.AliceBlue);
            var userCompanyClaim = User.FindFirst("CompanyId")?.Value;

            if (userRole == "SystemManager")
            {
                var allOperators = await _operatorService.GetAllAsync();
                return Ok(allOperators);
            }

            if(userRole == "Admin")
            {          
            if (int.TryParse(userCompanyClaim, out int companyId))
            {
                var companyOperators = await _operatorService.GetByCompanyIdAsync(companyId);
                return Ok(companyOperators);
            }
            }
            //else
            return Forbid("אין לך הרשאה לצפות בנתונים אלו.");
        }

        // GET api/<OperatorController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]

        public async Task<OperatorDto> Get(int id)
        {
            return await service.GetByIdAsync(id);
        }

        // POST api/<OperatorController>
        [HttpPost]
        [Authorize(Roles = "Admin,SystemManager")]

        public async Task<IActionResult> Post([FromBody] OperatorDto op)
        {
            var isexist =(await service.GetAllAsync())?.Any(x=>x.Mail==op.Mail);

            if (isexist ==true)
                throw new DuplicateException("האימייל הזה כבר קיים במערכת, עליך לבחור מייל אחר.");
            var newop = await service.AddAsync(op);
            return Ok(newop);
        }

        // PUT api/<OperatorController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]

        public async Task<IActionResult> Put(int id, [FromBody] OperatorDto op)
        {
            var isexist = (await service.GetAllAsync())?.Any(x => x.Mail == op.Mail);

            if (isexist == true)
                throw new DuplicateException("האימייל הזה כבר קיים במערכת, עליך לבחור מייל אחר.");
            var newop = await service.UpdateAsync(id, op);
            return Ok(newop);
        }

        // DELETE api/<OperatorController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SystemManager")]

        public async Task<IActionResult> Delete(int id)
        {
            await service.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] Login loginData)
        {

            //if the user existing ang the passworad is correctly
            var o = _operatorService.Exist(loginData);
            // 2. if exist
            if (o == null)
            {
                
                return Unauthorized("אימייל או סיסמה שגויים");
            }
            // 2. if valey return token
            var token = _operatorService.GenerateToken(o);//, "User"
            return Ok(new { token = token, user = o });
        }

        [HttpGet("GetAllMonthScore/{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]
        public async Task<IActionResult> GetAllMonthScore(int id)
        {
            var scores = await operatorService.GetAllMonthScoreAsync(id);
            return Ok(scores);
        }

        [HttpGet("GetMonthlyImprovement/{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]

        public async Task<IActionResult> GetMonthlyImprovement(int id)
        {
            var scores = await operatorService.GetMonthlyImprovementAsync(id);
            return Ok(scores);
        }

        [HttpGet("GetDalyImprovementTips/{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]

        public async Task<IActionResult> GetDalyImprovementTips(int id)
        {
            var scores = await operatorService.GetDailyImprovementTips(id);
            return Ok(scores);
        }
        [HttpGet("GetAverageDayScore/{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]

        public async Task<IActionResult> GetAverageDayScore(int id)
        {
            var scores = await operatorService.GetAverageDayScoreAsync(id);
            return Ok(scores);
        }

        [HttpGet("GetWeeklyImprovement/{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]

        public async Task<IActionResult> GetWeeklyImprovement(int id)
        {
            var scores = await operatorService.GetWeeklyImprovementAsync(id);
            return Ok(scores);
        }

        [HttpGet("GetAlldayScore/{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]

        public async Task<IActionResult> GetAlldayScore(int id)
        {
            var scores = await operatorService.GetAllDayScoreAsync(id);
            return Ok(scores);
        }

        [HttpGet("GetAllWeekScore/{id}")]
        [Authorize(Roles = "Admin,SystemManager,Operator")]

        public async Task<IActionResult> GetAllWeekScore(int id)
        {
            var scores = await operatorService.GetAllWeekScoreAsync(id);
            return Ok(scores);
        }

    }
}
