using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Common.Dto;
using System.Diagnostics.Metrics;


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

        public async Task<IEnumerable<Operator>> GetByCompanyIdAsync(int id)
        {
            return await ctx.Operators.Where(x => x.CompanyId == id).ToListAsync();
        }


        public async Task<Operator> UpdateAsync(int id, Operator item)
        {
            await ctx.SaveChangesAsync();
            return item;
        }

        //retrieving all scores for all calls -- gonna be weekly
        public async Task<IEnumerable<Score>> GetAllMonthScoreAsync(int id)
        {
            // שליפת הציונים של כל השיחות השייכות למפעיל הספציפי
            var scores = await ctx.Calls
                .Where(c => c.OperatorId == id && c.Score != null)
                .OrderBy(c => c.CallDate)
                .Select(c => c.Score) // כאן השינוי: אנחנו בוחרים את אובייקט ה-Score המלא
                .ToListAsync();

            return scores;
        }
        public async Task<IEnumerable<DailyOperatorDto>> GetAllWeekScoreAsync(int id, DateTime? todayy = null)
        {
            var today = todayy ?? DateTime.Today;
            var tomorrow = today.AddDays(1);
            var oneWeekAgo = today.AddDays(-7); // הגדרת נקודת ההתחלה - שבוע אחורה

            // ספירת כל השיחות של הטלפנית במהלך כל השבוע החולף
            var countOfWeeklyCalls = await ctx.Calls
                .Where(c => c.OperatorId == id
                       && c.Score != null
                       && c.CallDate >= oneWeekAgo
                       && c.CallDate < tomorrow)
                .CountAsync();

            return await ctx.Calls
                .Where(c => c.OperatorId == id
                       && c.Score != null
                       && c.CallDate >= oneWeekAgo
                       && c.CallDate < tomorrow)
                .Select(g => new DailyOperatorDto // שימוש ב-DTO הקיים
                {
                    OperatorToneScore = g.Score.OperatorToneScore,
                    ConflictResolutionScore = g.Score.ConflictResolutionScore,
                    ProfessionalismScore = g.Score.ProfessionalismScore,
                    OverallScore = g.Score.OverallScore,
                    ScoreId = g.Score.ScoreId,
                    GeneralNotes=g.GeneralNotes,
                    // שימי לב: השדה הזה יציג כעת את סך כל השיחות השבועיות 
                    SumDailyCalls = countOfWeeklyCalls,

                    // שליפת יום בשבוע דינמי לפי תאריך השיחה עצמה
                    DayName = g.CallDate.DayOfWeek
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<DailyOperatorDto>> GetAlldayScoreAsync(int id, DateTime? todayy = null)
        {
            var today = todayy ?? DateTime.Today;
            var tomorrow = today.AddDays(1);
            var countOfCalls = await ctx.Calls
                .Where(c => c.OperatorId == id
                       && c.Score != null
                       && c.CallDate >= today
                       && c.CallDate < tomorrow).CountAsync();
            return await ctx.Calls
                .Where(c => c.OperatorId == id
                       && c.Score != null
                       && c.CallDate >= today
                       && c.CallDate < tomorrow)
                .Select(g => new DailyOperatorDto // יצירה ישירה של ה-DTO
                {                    
                    OperatorToneScore = g.Score.OperatorToneScore,
                    ConflictResolutionScore = g.Score.ConflictResolutionScore,
                    ProfessionalismScore = g.Score.ProfessionalismScore,
                    OverallScore = g.Score.OverallScore,
                    SumDailyCalls = countOfCalls,
                    ScoreId= g.Score.ScoreId,
                    GeneralNotes = g.GeneralNotes,
                    DayName = new DateTime(today.Year, today.Month, today.Day).DayOfWeek
                })
                .ToListAsync();
        }
        //retrieving all scores for the past month
        public async Task<IEnumerable<object>> GetMonthlyImprovementAsync(int id)
        {
            var monthlyData = await ctx.Calls
                .Where(c => c.OperatorId == id && c.Score != null)
                .GroupBy(c => new { c.CallDate.Year, c.CallDate.Month }) // קיבוץ לפי שנה וחודש
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    // יצירת פורמט תאריך קריא לגרף, למשל "05/2026"
                    Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                    AvgTone = g.Average(x => x.Score.OperatorToneScore),
                    AvgConflict = g.Average(x => x.Score.ConflictResolutionScore),
                    AvgProfessionalism = g.Average(x => x.Score.ProfessionalismScore),
                    AvgOverall = g.Average(x => x.Score.OverallScore)
                })
                .ToListAsync();

            return monthlyData;
        }


        public async Task<IEnumerable<DailyOperatorDto>> GetAverageDayScoreAsync(int id, DateTime? todayy = null)
        {
            var today = todayy ?? DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await ctx.Calls
                .Where(c => c.OperatorId == id
                       && c.Score != null
                       && c.CallDate >= today
                       && c.CallDate < tomorrow)
                .GroupBy(c => new { c.CallDate.Year, c.CallDate.Month, c.CallDate.Day })
                .Select(g => new DailyOperatorDto // יצירה ישירה של ה-DTO
                {
                    //Day = g.Key.Day + "/" + g.Key.Month,
                    OperatorToneScore = g.Average(x => x.Score.OperatorToneScore),
                    ConflictResolutionScore = g.Average(x => x.Score.ConflictResolutionScore),
                    ProfessionalismScore = g.Average(x => x.Score.ProfessionalismScore),
                    OverallScore = g.Average(x => x.Score.OverallScore),
                    SumDailyCalls = g.Count(),
                    DayName = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day).DayOfWeek
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<object>> GetDailyImprovementTips(int id)
        {
            // הגדרת תאריך היעד מראש משפרת לעיתים ביצועים ב-EF
            var today = DateTime.Today;

            var dailyData = await ctx.Calls
               .Where(c => c.OperatorId == id
                        && c.Score != null
                        && c.CallDate.Date == today) // השוואת תאריך בלבד
               .Select(g => g.Score.ImprovementTips) // שליפת הערך עצמו
               .ToListAsync();

            return dailyData.Cast<object>(); // המרה ל-object כדי להתאים לחתימת הפונקציה
        }

        public async Task<IEnumerable<DailyOperatorDto>> GetWeeklyImprovementAsync(int id)
        {
            var today = DateTime.Today;
            var weeklist = new List<DailyOperatorDto>();
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
