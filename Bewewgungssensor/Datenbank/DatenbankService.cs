using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Bewewgungssensor.Datenbank
{
    public  class DatenbankService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatenbankService()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "highscores.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<HighScore>().Wait();
        }

        public async Task<List<HighScore>> GetHighscoresAsync()
        {
            return await _database.Table<HighScore>().OrderByDescending(h => h.Score).ToListAsync();
        }

        public async Task AddHighscoreAsync(string spielername, int score)
        {
            await _database.InsertAsync(new HighScore { Spielername = spielername, Score = score });

            // Stelle sicher, dass nur die 10 höchsten Scores bleiben
            var highscores = await GetHighscoresAsync();
            if (highscores.Count > 10)
            {
                var lowScore = highscores.Last();
                await _database.DeleteAsync(lowScore);
            }
        }
    }
}
