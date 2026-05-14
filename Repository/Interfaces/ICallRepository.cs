using Common.Dto;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICallRepository<T>
    {
        Task<IEnumerable<Call>> GetByIdOperatorAsync(int id);
        Task<ScoreCompanyDto> GetDailyImprovementAsync(int id);

    }
}
