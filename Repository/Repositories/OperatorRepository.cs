using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Repository.Repositories
{
    public class OperatorRepository : IRepository<Operator>,IOperatorRepository
    {

        private readonly IContext ctx;
        public OperatorRepository(IContext context)
        {
            this.ctx = context;
        }
        public async Task<Operator> AddAsync(Operator item)
        {
            ctx.Operators.Add(item);
            await  ctx.SaveChangesAsync();
            return item;
        }

        public async Task DeleteAsync(int id)
        {
            await ctx.Operators.Where(x => x.OperatorId == id).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Operator>> GetAllAsync()
        {
            return await ctx.Operators.ToListAsync();
        }

        public async Task<Operator> GetByIdAsync(int id)
        {
            return await ctx.Operators.FirstOrDefaultAsync(x => x.OperatorId == id);

        }

        public async Task<Operator> UpdateAsync(int id, Operator item)
        {
            await ctx.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<object>> GetAllMonthScoreAsync(int id)
        {
            // שליפת המפעיל כולל הנתונים הקשורים
            var scores = await ctx.Calls
                .Where(c => c.OperatorId == id && c.Score != null)
                .OrderBy(c => c.CallDate)
                .Select(c => new
                {
                    Date = c.CallDate,
                    Score = c.Score.OverallScore
                })
                .ToListAsync(); // כאן מתבצעת הפעולה האסינכרונית מול ה-DB

            return scores;
        }
    }
}
