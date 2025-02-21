using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FotoApp.MVVM.Model;
using SQLite;

namespace FotoApp.MVVM.Data
{
    public class Constants
    {
        private readonly SQLiteAsyncConnection _database;

    public Constants(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            try
            {

                _database.CreateTableAsync<Assignment>().Wait();
                _database.CreateTableAsync<Comment>().Wait();
                _database.CreateTableAsync<User>().Wait();
                _database.CreateTableAsync<Photo>().Wait();
                _database.CreateTableAsync<Transaction>().Wait();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tables: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }

        }
        public Task<List<T>> GetAllAsync<T>() where T : new()
        {
            return _database.Table<T>().ToListAsync();
        }

        public Task<int> AddAsync<T>(T item) where T : new()
        {
            return _database.InsertAsync(item);
        }

        public Task<int> UpdateAsync<T>(T item) where T : new()
        {
            return _database.UpdateAsync(item);
        }

        public Task<int> DeleteAsync<T>(T item) where T : new()
        {
            return _database.DeleteAsync(item);
        }

        public Task<int> AddAllAsync<T>(IEnumerable<T> items) where T : new()
        {
            return _database.InsertAllAsync(items);
        }
        public Task<T> GetAsync<T>(int id) where T : new()
        {
            return _database.FindAsync<T>(id);
        }
    }
}
