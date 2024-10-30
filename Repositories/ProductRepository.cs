using Dapper;
using Microsoft.Data.Sqlite;
using ProductCatalogApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCatalogApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SqliteConnection _connection;

        public ProductRepository(SqliteConnection connection)
        {
            _connection = connection;
            _connection.Open();
            var sql = "CREATE TABLE IF NOT EXISTS Products (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Price REAL)";
            _connection.Execute(sql);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _connection.QueryAsync<Product>("SELECT * FROM Products");
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Products WHERE Id = @Id", new { Id = id });
        }

        public async Task<Product> CreateAsync(Product product)
        {
            var sql = "INSERT INTO Products (Name, Price) VALUES (@Name, @Price); SELECT last_insert_rowid();";
            product.Id = await _connection.ExecuteScalarAsync<int>(sql, product);
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            var sql = "UPDATE Products SET Name = @Name, Price = @Price WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, product);
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Products WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
