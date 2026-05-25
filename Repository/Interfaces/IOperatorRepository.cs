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
        Task<IEnumerable<Operator>> GetByCompanyIdAsync(int id);

        Task<IEnumerable<Score>> GetAllMonthScoreAsync(int id);

        Task<IEnumerable<object>> GetMonthlyImprovementAsync(int id);

        Task<IEnumerable<DailyOperatorDto>> GetAverageDayScoreAsync(int id, DateTime? todayy = null);

        Task<IEnumerable<object>> GetDailyImprovementTips(int id);

        Task<IEnumerable<DailyOperatorDto>> GetWeeklyImprovementAsync(int id);

        Task<IEnumerable<DailyOperatorDto>> GetAllWeekScoreAsync(int id, DateTime? todayy = null);

        Task<IEnumerable<DailyOperatorDto>> GetAlldayScoreAsync(int id, DateTime? todayy = null);



    }
}
