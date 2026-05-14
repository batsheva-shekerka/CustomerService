using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICompanyRepository
    {
        //Task<IEnumerable<ScoreDto>> GetDailyImprovementAsync(int id);
        Task<IEnumerable<ScoreDto>> GetWeeklyImprovementAsync(int id);
        Task<IEnumerable<ScoreDto>> GetAverageDayScoreAsync(int id, DateTime? todayy = null);



    }
}
