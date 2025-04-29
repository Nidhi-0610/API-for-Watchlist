using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;
using System.Threading.Tasks;
using StockWatchlist.Repositories;

namespace StockWatchlist.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public StockService(IStockRepository stockRepository, HttpClient httpClient, string apiKey)
        {
            _stockRepository = stockRepository;
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public void AddFavoriteStock(string stock)
        {
            _stockRepository.AddStock(stock);
        }

        public IEnumerable<string> GetFavoriteStocks()
        {
            return _stockRepository.GetFavoriteStocks();
        }

        public async Task<object> FetchStockDataAsync(string stockSymbol)
        {
            var relativeUrl = $"/quote?symbol={stockSymbol}&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(relativeUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Error: {response.StatusCode} - {response.ReasonPhrase}");
            }

            var rawData = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonNode.Parse(rawData);

            if (jsonObject == null)
            {
                throw new Exception("Invalid data from Twelve Data API.");
            }

            decimal ParseDecimal(string value) =>
                decimal.TryParse(value, out var result) ? result : 0.0M;

            long ParseLong(string value) =>
                long.TryParse(value, out var result) ? result : 0;

            return new
            {
                Symbol = jsonObject["symbol"]?.ToString() ?? "Unknown Symbol",
                Name = jsonObject["name"]?.ToString() ?? "Unknown Company",
                CurrentPrice = ParseDecimal(jsonObject["price"]?.ToString() ?? "0.0"),
                PercentChange = ParseDecimal(jsonObject["percent_change"]?.ToString() ?? "0.0"),
                Open = ParseDecimal(jsonObject["open"]?.ToString() ?? "0.0"),
                High = ParseDecimal(jsonObject["high"]?.ToString() ?? "0.0"),
                Low = ParseDecimal(jsonObject["low"]?.ToString() ?? "0.0"),
                Close = ParseDecimal(jsonObject["close"]?.ToString() ?? "0.0"),
                Volume = ParseLong(jsonObject["volume"]?.ToString() ?? "0")
            };
        }

        public async Task<List<object>> FetchStockDataForMultipleSymbolsAsync(List<string> stockSymbols)
        {
            var tasks = stockSymbols.Select(async symbol =>
            {
                try
                {
                    return await FetchStockDataAsync(symbol);
                }
                catch (Exception ex)
                {
                    return new
                    {
                        Symbol = symbol,
                        Error = $"Failed to fetch data: {ex.Message}"
                    };
                }
            });

            return (await Task.WhenAll(tasks)).ToList();
        }
    }
}
