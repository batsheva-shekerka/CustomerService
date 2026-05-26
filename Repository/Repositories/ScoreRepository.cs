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
    public class ScoreRepository : IRepository<Score>
    {

        private readonly IContext ctx;
        public ScoreRepository(IContext context)
        {
            this.ctx = context;
        }
        public async Task<Score> AddAsync(Score item)
        {
            ctx.Scores.Add(item);
            await ctx.SaveChangesAsync();
            return item;
        }

        public async Task DeleteAsync(int id)
        {
            await ctx.Scores.Where(x => x.ScoreId == id).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Score>> GetAllAsync()
        {
            return await ctx.Scores.ToListAsync();
        }

        public async Task<Score> GetByIdAsync(int id)
        {
            return await ctx.Scores.FirstOrDefaultAsync(x => x.ScoreId == id);

        }

        public async Task<Score> UpdateAsync(int id, Score item)
        {
            ctx.Scores.Update(item);
            await ctx.SaveChangesAsync();
            return item;
        }      
    }

}
