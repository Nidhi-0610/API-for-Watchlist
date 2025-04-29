using System.Collections.Generic;

namespace StockWatchlist.Repositories
{
    public class StockRepository : IStockRepository
    {
        private static readonly List<string> _favoriteStocks = new();

        public void AddStock(string stock)
        {
            if (!string.IsNullOrWhiteSpace(stock) && !_favoriteStocks.Contains(stock))
            {
                _favoriteStocks.Add(stock);
            }
        }

        public IEnumerable<string> GetFavoriteStocks()
        {
            return _favoriteStocks;
        }
    }
}
