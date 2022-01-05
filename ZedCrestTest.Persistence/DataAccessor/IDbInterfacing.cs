using System.Threading.Tasks;
using System.Data;
using Dapper;

namespace ZedCrestTest.Persistence.DataAccessor
{
     public interface IDbInterfacing
    {
         Task<SqlModelRes<T>> GetList<T>(string connString, string commandName, CommandType commandType, DynamicParameters param);
         Task<SqlModelRes<T>> GetItem<T>(string connString, string commandName, CommandType commandType, DynamicParameters param);
         Task<SqlModelRes<int>> ModifyDB(string connString, string commandName, CommandType commandType, DynamicParameters param);
    }
}