using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;
using Common.Dto;
using Service.Interfaces;
using Service.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Drawing;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerService.webApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatorController : ControllerBase
    {
        private readonly Iservice<OperatorDto> service;
        private readonly IOperatorService operatorService;
        //private readonly IRepository<Operator> repository;
        //private readonly IsExist<Operator> _isExist;
        private readonly OperatorService _operatorService;

        //private readonly IAuthService _authService;

        // כאן אנחנו מקבלים את הכל מהמערכת
        public OperatorController(Iservice<OperatorDto> service, OperatorService operatorService,IOperatorService operatorService1)//, IAuthService authService, IsExist<Operator> isExist
        {
            this.service = service;
            //this._isExist = isExist;
            this._operatorService = operatorService;
            this.operatorService = operatorService1;
            //this._authService = authService;
        }
        // GET: api/<OperatorController>
        [HttpGet]
        [Authorize]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            // 1. חילוץ התפקיד והחברה של המשתמש מתוך ה-Token (Claims)
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"DEBUG: User Role is: {userRole}",Color.AliceBlue);
            var userCompanyClaim = User.FindFirst("CompanyId")?.Value;

            // 2. בדיקה לוגית - אם המשתמש הוא מנהל מערכת, נביא את כל הטלפניות
            if (userRole == "SystemManager")
            {
                var allOperators = await _operatorService.GetAllAsync();
                return Ok(allOperators);
            }

            // 3. אם הוא מנהל חברה, נבדוק שיש לו מזהה חברה תקין ונחזיר רק את שלו
            if (int.TryParse(userCompanyClaim, out int companyId))
            {
                var companyOperators = await _operatorService.GetByCompanyIdAsync(companyId);
                return Ok(companyOperators);
            }

            // אם אין לו תפקיד מתאים או חברה תקינה - נחסום את הגישה
            return Forbid("אין לך הרשאה לצפות בנתונים אלו.");
        }

        // GET api/<OperatorController>/5
        [HttpGet("{id}")]
        public async Task<OperatorDto> Get(int id)
        {
            return await service.GetByIdAsync(id);
        }

        // POST api/<OperatorController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OperatorDto op)
        {
            var newop = await service.AddAsync(op);
            return Ok(newop);
        }

        // PUT api/<OperatorController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OperatorDto op)
        {
            var newop = await service.UpdateAsync(id, op);
            return Ok(newop);
        }

        // DELETE api/<OperatorController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await service.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] Login loginData)
        {

            // 1. בדיקה מול ה-Repository שהמשתמש קיים (דילגתי על זה לצורך הדוגמה)
            //האם המשתמש קיים וכן אימות הסיסמא
            var o = _operatorService.Exist(loginData);
            // 2. בדיקה האם המשתמש נמצא
            if (o == null)
            {
                // אם לא נמצא (סיסמה שגויה או מייל לא קיים), מחזירים שגיאה מוסדרת ולא קורסים
                return Unauthorized("אימייל או סיסמה שגויים");
            }
            // 2. אם המשתמש תקין - קוראים לסרביס לייצר טוקן
            var token = _operatorService.GenerateToken(o);//, "User"
            return Ok(new { token = token, user = o });
        }

        [HttpGet("GetAllMonthScore/{id}")]
        public async Task<IActionResult> GetAllMonthScore(int id)
        {
            var scores = await operatorService.GetAllMonthScoreAsync(id);
            return Ok(scores);
        }

        [HttpGet("GetMonthlyImprovement/{id}")]
        public async Task<IActionResult> GetMonthlyImprovement(int id)
        {
            var scores = await operatorService.GetMonthlyImprovementAsync(id);
            return Ok(scores);
        }

        [HttpGet("GetDalyImprovementTips/{id}")]
        public async Task<IActionResult> GetDalyImprovementTips(int id)
        {
            var scores = await operatorService.GetDailyImprovementTips(id);
            return Ok(scores);
        }
        [HttpGet("GetAverageDayScore/{id}")]
        public async Task<IActionResult> GetAverageDayScore(int id)
        {
            var scores = await operatorService.GetAverageDayScoreAsync(id);
            return Ok(scores);
        }

        [HttpGet("GetWeeklyImprovement/{id}")]
        public async Task<IActionResult> GetWeeklyImprovement(int id)
        {
            var scores = await operatorService.GetWeeklyImprovementAsync(id);
            return Ok(scores);
        }

        [HttpGet("GetAlldayScore/{id}")]
        public async Task<IActionResult> GetAlldayScore(int id)
        {
            var scores = await operatorService.GetAllDayScoreAsync(id);
            return Ok(scores);
        }

        [HttpGet("GetAllWeekScore/{id}")]
        public async Task<IActionResult> GetAllWeekScore(int id)
        {
            var scores = await operatorService.GetAllWeekScoreAsync(id);
            return Ok(scores);
        }

    }
}
