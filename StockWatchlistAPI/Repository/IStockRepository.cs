using System.Collections.Generic;

namespace StockWatchlist.Repositories
{
    public interface IStockRepository
    {
        void AddStock(string stock);
        IEnumerable<string> GetFavoriteStocks();
    }
}
