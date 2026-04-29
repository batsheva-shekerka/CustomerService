using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;
using Common.Dto;
using Service.Interfaces;
using Service.Services;
using Microsoft.AspNetCore.Authorization;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerService.webApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatorController : ControllerBase
    {
        private readonly Iservice<OperatorDto> service;
        //private readonly IRepository<Operator> repository;
        //private readonly IsExist<Operator> _isExist;
        private readonly OperatorService _operatorService;

        //private readonly IAuthService _authService;

        // כאן אנחנו מקבלים את הכל מהמערכת
        public OperatorController(Iservice<OperatorDto> service, OperatorService operatorService)//, IAuthService authService, IsExist<Operator> isExist
        {
            this.service = service;
            //this._isExist = isExist;
            _operatorService = operatorService;
            //this._authService = authService;
        }
        // GET: api/<OperatorController>
        [HttpGet]
       // [Authorize(Roles = "Admin")]
        public async  Task<IEnumerable<OperatorDto>> Get()
        {
            return await service.GetAllAsync();
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
            var newop= await service.AddAsync(op);
             return Ok(newop);
        }

        // PUT api/<OperatorController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OperatorDto op)
        {
           var newop= await service.UpdateAsync(id, op);
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
            var o= _operatorService.Exist(loginData);
            // 2. בדיקה האם המשתמש נמצא
            if (o == null)
            {
                // אם לא נמצא (סיסמה שגויה או מייל לא קיים), מחזירים שגיאה מוסדרת ולא קורסים
                return Unauthorized("אימייל או סיסמה שגויים");
            }
            // 2. אם המשתמש תקין - קוראים לסרביס לייצר טוקן
            var token = _operatorService.GenerateToken(o);//, "User"
            return Ok(new { token = token ,user=o });
        }
    }
}
