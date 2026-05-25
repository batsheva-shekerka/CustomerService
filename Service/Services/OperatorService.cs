using AutoMapper;
//using AutoMapper.Configuration;
using Common.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataContext;
using Common.Enums;
using Microsoft.CognitiveServices.Speech.Transcription;

namespace Service.Services
{
    public class OperatorService : Iservice<OperatorDto>,IsExist<Operator>, IOperatorService
    {
        private readonly IRepository<Operator> repository;
        private readonly IOperatorRepository operatorRepository;

        private readonly IMapper mapper;
        private readonly IConfiguration _configuration;
        //public readonly IsExist<OperatorDto> isExist;
        private readonly CustomerServiceContext _context;


        public OperatorService(IRepository<Operator> repository, IMapper map, IConfiguration configuration, CustomerServiceContext context,IOperatorRepository operatorRepository)// IsExist<OperatorDto> isExist,
        {
            this.repository = repository;
            this.operatorRepository=operatorRepository;
            this.mapper = map;
            this._configuration = configuration;
            //this.isExist = isExist;
            this._context = context;
        }

        public async Task<List<OperatorDto>> GetAllAsync()
        {          
            var rep = await repository.GetAllAsync();
            return  mapper.Map<List<OperatorDto>>(rep);
        }
        public Operator Exist(Login l)
        {
            var op = _context.Operators.FirstOrDefault(x => x.Mail == l.Email);
            if (op == null)               
                return null;
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(l.PasswordHash, op.PasswordHash);

            if (!isPasswordCorrect)
            {
                return null;            }
            return op;
        }

        //יצירת טוקן
        public string GenerateToken(Operator op)
        {
            if (op == null)
            {
                // טיפול במקרה של null - למשל זריקת שגיאה מפורטת או החזרת null
                throw new ArgumentNullException(nameof(op), "Operator cannot be null");
            }
            //המפתח להצפנה
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            //אלגוריתם להצפנה
            var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            //אובייקט שמכיל את נתוני המשתמש לפי מפתחות
            var claims = new[] {
            new Claim(ClaimTypes.Email,op.Mail),
            new Claim(ClaimTypes.Name,op.FirstName),
            new Claim(ClaimTypes.Surname,op.LastName),
            new Claim(ClaimTypes.Role, op.Role.ToString()),
            new Claim("CompanyId", op.CompanyId.ToString())  
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);

        }



        public async Task<OperatorDto> GetByIdAsync(int id) 
        {
            var operatorEntity = await repository.GetByIdAsync(id);

            return mapper.Map<OperatorDto>(operatorEntity);
        }
        public async Task<IEnumerable<OperatorDto>> GetByCompanyIdAsync(int id)
        {
            var operatorEntity = await operatorRepository.GetByCompanyIdAsync(id);

            return mapper.Map<List<OperatorDto>>(operatorEntity);
        }
        public async Task<OperatorDto> AddAsync(OperatorDto item) 
        {
            string passwordFromUser = item.PasswordHash;

            // יצירת ה-Hash (ההצפנה)
            string hashedPath = BCrypt.Net.BCrypt.HashPassword(passwordFromUser);

            // עכשיו שומרים את hashedPath בבסיס הנתונים

            item.PasswordHash = hashedPath;
           
            var operatorEntity = await repository.AddAsync(mapper.Map<Operator>(item));

            return mapper.Map<OperatorDto>(operatorEntity);
        }
       public async Task<OperatorDto> UpdateAsync(int id, OperatorDto item) 
        {
            var op = _context.Operators.FirstOrDefault(x => x.OperatorId == id);
            if (op!=null)
            {
                string passwordFromUser = item.PasswordHash;
                string hashedPath = BCrypt.Net.BCrypt.HashPassword(passwordFromUser);
                item.PasswordHash = hashedPath;
                mapper.Map(item, op);
                var operatorEntity = await repository.UpdateAsync(id, op);
                return mapper.Map<OperatorDto>(operatorEntity);
            }
            return null;
        }
       public async Task DeleteAsync(int id) 
        {
            await repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ScoreDto>> GetAllMonthScoreAsync(int id)
        {
           var scores = await operatorRepository.GetAllMonthScoreAsync(id);
            return mapper.Map<List<ScoreDto>>(scores);
        }
        public async  Task<IEnumerable<object>> GetMonthlyImprovementAsync(int id)
        {
            var scores = await operatorRepository.GetAllMonthScoreAsync(id);
            return mapper.Map<List<ScoreDto>>(scores);
        }

        public async Task<IEnumerable<DailyOperatorDto>> GetAverageDayScoreAsync(int id)
        {
            var scores = await operatorRepository.GetAverageDayScoreAsync(id);
            return scores.ToList(); // אין צורך ב-mapper.Map
        }

        public async Task<List<ImprovementTips>> GetDailyImprovementTips(int id)
        {
            var scores = await operatorRepository.GetDailyImprovementTips(id);
            return mapper.Map<List<ImprovementTips>>(scores);
        }

        public async Task<IEnumerable<object>> GetWeeklyImprovementAsync(int id)
        {
            var scores = await operatorRepository.GetWeeklyImprovementAsync(id);
            return scores.ToList();
            //return mapper.Map<List<DailyOperatorDto>>(scores);
        }

        public async Task<IEnumerable<DailyOperatorDto>> GetAllWeekScoreAsync(int id)
        {
            var scores = await operatorRepository.GetAllWeekScoreAsync(id);
            return scores.ToList(); // אין צורך ב-mapper.Map
        }
        public async Task<IEnumerable<DailyOperatorDto>> GetAllDayScoreAsync(int id)
        {
            var scores = await operatorRepository.GetAlldayScoreAsync(id);
            return scores.ToList(); // אין צורך ב-mapper.Map
        }


    }
}