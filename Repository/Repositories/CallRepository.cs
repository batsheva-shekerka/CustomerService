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
    public class CallRepository : IRepository<Call>,ICallRepository<Call>
    {

        private readonly IContext ctx;
        public CallRepository(IContext context)
        {
            this.ctx = context;
        }
        public async Task<Call> AddAsync(Call item)
        {
            ctx.Calls.Add(item);
            await ctx.SaveChangesAsync();
            return item;
        }

        public async Task DeleteAsync(int id)
        {
            await ctx.Calls.Where(x => x.CallId == id).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Call>> GetAllAsync()
        {
            return await ctx.Calls.ToListAsync();
        }

        public async Task<Call> GetByIdAsync(int id)
        {
            return await ctx.Calls.FirstOrDefaultAsync(x => x.CallId == id);

        }

        public async Task<Call> UpdateAsync(int id, Call item)
        {
            ctx.Calls.Update(item);
            await ctx.SaveChangesAsync();
            return item;
        }


        public async Task<IEnumerable<Call>> GetByIdOperatorAsync(int id)
        {
            return await ctx.Calls.Where(x=>x.OperatorId==id).ToListAsync();

        }

       
    }

}
