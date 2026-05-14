using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Common.Dto;

namespace Repository.Repositories
{
    public class CompanyRepository : IRepository<Company>, ICompanyRepository
    {

        private readonly IContext ctx;
        public CompanyRepository(IContext context)
        {
            this.ctx = context;
        }
        public async Task<Company> AddAsync(Company item)
        {
            ctx.Companies.Add(item);
           await  ctx.SaveChangesAsync();
            return item;
        }

        public async Task DeleteAsync(int id)
        {
            await ctx.Companies.Where(x => x.CompanyId == id).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await ctx.Companies.ToListAsync();
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            return await ctx.Companies.FirstOrDefaultAsync(x => x.CompanyId == id);

        }

        public async Task<Company> UpdateAsync(int id, Company item)
        {
            ctx.Companies.Update(item);
            await ctx.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<ScoreDto>> GetAverageDayScoreAsync(int id, DateTime? todayy = null)
        {
            var today = todayy ?? DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await ctx.Calls
                .Where(c => c.CompanyId == id
                       && c.Score != null
                       && c.CallDate >= today
                       && c.CallDate < tomorrow)
                .GroupBy(c => new { c.CallDate.Year, c.CallDate.Month, c.CallDate.Day })
                .Select(g => new ScoreDto // יצירה ישירה של ה-DTO
                {
                    //Day = g.Key.Day + "/" + g.Key.Month,
                    OperatorToneScore = g.Average(x => x.Score.OperatorToneScore),
                    ConflictResolutionScore = g.Average(x => x.Score.ConflictResolutionScore),
                    ProfessionalismScore = g.Average(x => x.Score.ProfessionalismScore),
                    OverallScore = g.Average(x => x.Score.OverallScore)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ScoreDto>> GetWeeklyImprovementAsync(int id)
        {
            var today = DateTime.Today;
            var weeklist = new List<ScoreDto>();
            for (int i = 0; i < 7; i++)
            {
                var day = await GetAverageDayScoreAsync(id, today);
                today = today.AddDays(-1);
                weeklist.AddRange(day);
            }
            return weeklist;
        }




    }

}
