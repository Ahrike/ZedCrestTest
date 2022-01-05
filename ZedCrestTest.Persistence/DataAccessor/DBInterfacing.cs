
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace ZedCrestTest.Persistence.DataAccessor
{
    public class DbInterfacing : IDbInterfacing
    {
        private ILogger<DbInterfacing> _logger;
        public DbInterfacing(ILogger<DbInterfacing> logger)
        {
            _logger = logger;
        }
        public async Task<SqlModelRes<T>> GetList<T>(string connString, string commandName, CommandType commandType, DynamicParameters param)
        {
            var res = new SqlModelRes<T>();
            string classMeth = "GetList";
            try
            {
                using (var connection = new MySqlConnection(connString))
                {
                    var result = await connection.QueryAsync<T>(commandName, param, null, null, commandType);
                    if (result != null)
                    {
                        res.Success = true;
                    }
                    res.ResultList = result.AsList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(classMeth + "An Error occured while executing " + commandName + " " + ex.Message + " " + ex.StackTrace);
                res.Success = false;
                res.ErrorMessage = ex.ToString();
            }
            return res;
        }

        public async Task<SqlModelRes<T>> GetItem<T>(string connString, string commandName, CommandType commandType, DynamicParameters param)
        {
            var res = new SqlModelRes<T>();
            string classMeth = "GetItem";
            try
            {
                using (var connection = new MySqlConnection(connString))
                {
                    var result = await connection.QuerySingleOrDefaultAsync<T>(commandName, param, null, null, commandType);
                    if (result != null)
                    {
                        res.Success = true;
                    }
                    res.Result = result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(classMeth + "An Error occured while executing " + commandName + " " + ex.Message + " " + ex.StackTrace);
                res.Success = false;
                res.ErrorMessage = ex.ToString();
            }
            return res;
        }
        public async Task<SqlModelRes<int>> ModifyDB(string connString, string commandName, CommandType commandType, DynamicParameters param)
        {
            var res = new SqlModelRes<int>();
            string classMeth = "ModifyDB";
            try
            {
                using (var connection = new MySqlConnection(connString))
                {

                    var result = await connection.ExecuteAsync(commandName, param, null, null, commandType);
                    if (result > 0)
                    {
                        res.Success = true;
                    }
                    res.ResultInt = result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(classMeth + "An Error occured while executing " + commandName + " " + ex.Message + " " + ex.StackTrace);
                res.Success = false;
                res.ErrorMessage = ex.ToString();
            }
            return res;
        }
    }
}