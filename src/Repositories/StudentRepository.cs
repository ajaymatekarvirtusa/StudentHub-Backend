using Dapper;
using Data.Models;
using Microsoft.Extensions.Logging;
using Repositories.DBContext;
using Repositories.Interface;
using System.Data;
using System.Diagnostics;

namespace Repositories
{
    public class StudentRepository(StudentDbContext _dbContext, ILogger<StudentRepository> _logger) : IStudentRepository
    {
        private const string StoredProcedure = "sp_StudentDetail";

        public Task<int> AddStudentAsync(StudentDetail studentDetail)
        {
            var parameters = StudentParameters(studentDetail);
            parameters.Add("@CreatedBy", studentDetail.CreatedBy);

            return ExecuteAsync("INSERT", null, parameters, (db, command) => db.ExecuteScalarAsync<int>(command));
        }

        public Task<StudentDetail?> GetStudentByIdAsync(int studentId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@StudentId", studentId);

            return ExecuteAsync("GETBYID", studentId, parameters, (db, command) => db.QueryFirstOrDefaultAsync<StudentDetail>(command));
        }

        public Task<IEnumerable<StudentDetail>> GetAllStudentsAsync()
        {
            return ExecuteAsync("GETALL", null, new DynamicParameters(), (db, command) => db.QueryAsync<StudentDetail>(command));
        }

        public Task<int> UpdateStudentAsync(StudentDetail studentDetail)
        {
            var parameters = StudentParameters(studentDetail);
            parameters.Add("@Id", studentDetail.Id);
            parameters.Add("@ModifiedBy", studentDetail.ModifiedBy);

            return ExecuteAsync("UPDATE", studentDetail.Id, parameters, (db, command) => db.ExecuteScalarAsync<int>(command));
        }

        public async Task<bool> DeleteStudentAsync(int studentId, int modifiedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@StudentId", studentId);
            parameters.Add("@ModifiedBy", modifiedBy);

            var result = await ExecuteAsync("DELETE", studentId, parameters, (db, command) => db.ExecuteScalarAsync<int>(command));
            return result == 1;
        }

        /// <summary>Fields shared by INSERT and UPDATE.</summary>
        private static DynamicParameters StudentParameters(StudentDetail studentDetail)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Name", studentDetail.Name);
            parameters.Add("@Dob", studentDetail.Dob);
            parameters.Add("@MobileNo", studentDetail.MobileNo);
            return parameters;
        }

        /// <summary>
        /// Adds @Action, runs sp_StudentDetail, and logs timing and errors.
        /// Only ids are logged - never personal data such as name, DOB or mobile number.
        /// </summary>
        private async Task<T> ExecuteAsync<T>(string action, int? studentId, DynamicParameters parameters,
            Func<IDbConnection, CommandDefinition, Task<T>> query)
        {
            parameters.Add("@Action", action, DbType.String);
            var command = new CommandDefinition(StoredProcedure, parameters, commandType: CommandType.StoredProcedure);

            var stopwatch = Stopwatch.StartNew();
            _logger.LogDebug("Executing {StoredProcedure} Action={Action} StudentId={StudentId}", StoredProcedure, action, studentId);

            try
            {
                var result = await query(_dbContext.Connection, command);
                stopwatch.Stop();

                _logger.LogDebug("Executed {StoredProcedure} Action={Action} StudentId={StudentId} in {ElapsedMs} ms",
                    StoredProcedure, action, studentId, stopwatch.ElapsedMilliseconds);

                if (stopwatch.ElapsedMilliseconds > 2000)
                {
                    _logger.LogWarning("Slow query: {StoredProcedure} Action={Action} took {ElapsedMs} ms",
                        StoredProcedure, action, stopwatch.ElapsedMilliseconds);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in {StoredProcedure} Action={Action} StudentId={StudentId} after {ElapsedMs} ms",
                    StoredProcedure, action, studentId, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
