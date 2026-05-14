using Common.Dto;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IOperatorService
    {
        Task<IEnumerable<ScoreDto>> GetAllMonthScoreAsync(int id);

        Task<IEnumerable<object>> GetMonthlyImprovementAsync(int id);
        Task<IEnumerable<object>> GetAverageDayScoreAsync(int id);
        Task<List<ImprovementTips>> GetDailyImprovementTips(int id);
        Task<IEnumerable<object>> GetWeeklyImprovementAsync(int id);

    }
}
