using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace OrderProcessingDAL
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly string _connectionString;
        public GenericRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
 
       
        public IEnumerable<T> GetAll()
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var tableName = typeof(T).Name;
                return dbConnection.Query<T>($"SELECT * FROM {tableName}s");
            }
          
        }

        public T? GetById(int id)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var tableName = typeof(T).Name;
                return dbConnection.Query<T>($"SELECT * FROM {tableName}s WHERE Id = @Id", new { Id = id }).FirstOrDefault();
            }
        }

        public virtual void Add(T entity)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var tableName = typeof(T).Name;
                var properties = typeof(T).GetProperties().Where(p => p.Name != "Id").Select(p => p.Name);
                var columns = string.Join(", ", properties);
                var values = string.Join(", ", properties.Select(p => "@" + p));
                var sql = $"INSERT INTO {tableName}s ({columns}) VALUES ({values})";
                dbConnection.Execute(sql, entity);
            }
        }

        public void Update(T entity)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var tableName = typeof(T).Name;
                var properties = typeof(T).GetProperties().Where(p => p.Name != "Id").Select(p => p.Name);
                var setClause = string.Join(", ", properties.Select(p => $"{p} = @{p}"));
                var sql = $"UPDATE {tableName}s SET {setClause} WHERE Id = @Id";
                dbConnection.Execute(sql, entity);
            }
        }

        public void Delete(int id)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var tableName = typeof(T).Name;
                var sql = $"DELETE FROM {tableName}s WHERE Id = @Id";
                dbConnection.Execute(sql, new { Id = id });
            }
        }
    }
}