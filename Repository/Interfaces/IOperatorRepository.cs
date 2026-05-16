using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Dto;

namespace Repository.Interfaces
{
    public interface IOperatorRepository
    {
        Task<IEnumerable<Score>> GetAllMonthScoreAsync(int id);

        Task<IEnumerable<object>> GetMonthlyImprovementAsync(int id);

        Task<IEnumerable<DailyOperatorDto>> GetAverageDayScoreAsync(int id, DateTime? todayy = null);

        Task<IEnumerable<object>> GetDailyImprovementTips(int id);

        Task<IEnumerable<DailyOperatorDto>> GetWeeklyImprovementAsync(int id);



    }
}
