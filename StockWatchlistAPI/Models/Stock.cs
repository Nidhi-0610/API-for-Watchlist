namespace StockWatchlistAPI.Models
{
    public class Stock
    {
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public Stock(string symbol, string name)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol)); // Ensure it's never null
            Name = name ?? throw new ArgumentNullException(nameof(name)); // Ensure it's never null
        }
    }
}
