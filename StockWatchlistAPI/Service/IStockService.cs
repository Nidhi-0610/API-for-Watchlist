using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWatchlist.Services
{
    public interface IStockService
    {
        void AddFavoriteStock(string stock);
        IEnumerable<string> GetFavoriteStocks();
        Task<object> FetchStockDataAsync(string stockSymbol);
        Task<List<object>> FetchStockDataForMultipleSymbolsAsync(List<string> stockSymbols);
    }
}
