using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IScoreRepository
    {
        Task<IEnumerable<ScoreDto>> GetDailyImprovementAsync(int id);

    }
}
