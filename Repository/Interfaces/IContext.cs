using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository.Interfaces
{
    public interface IContext
    {
        DbSet<Company> Companies { get; set; }
        DbSet<Operator> Operators { get; set; }
        DbSet<Call> Calls { get; set; }
        //DbSet<CallParticipantAnalysis> CallParticipantAnalyses { get; set; }
        DbSet<Score> Scores { get; set; }

        // פונקציה לשמירת שינויים
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
