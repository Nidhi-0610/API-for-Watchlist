using Microsoft.AspNetCore.Mvc;
using StockWatchlist.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockWatchlist.Controllers
{
    [ApiController]
    [Route("api/stocks")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost("add")]
        public IActionResult AddFavoriteStock([FromQuery] string stockSymbol)
        {
            if (string.IsNullOrWhiteSpace(stockSymbol))
            {
                return BadRequest("Stock symbol is required.");
            }

            _stockService.AddFavoriteStock(stockSymbol);
            return Ok(new
            {
                Message = $"Stock {stockSymbol} added to favorites!",
                FavoriteStocks = _stockService.GetFavoriteStocks()
            });
        }

        [HttpGet("favorites")]
        public IActionResult GetFavoriteStocks()
        {
            var favoriteStocks = _stockService.GetFavoriteStocks();
            if (!favoriteStocks.Any())
            {
                return Ok("No favorite stocks added.");
            }

            return Ok(favoriteStocks);
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> FetchStockData([FromQuery] string stockSymbols)
        {
            List<string> symbols;

            if (string.IsNullOrWhiteSpace(stockSymbols))
            {
                // Retrieve stock symbols directly from favorites if none are provided
                symbols = _stockService.GetFavoriteStocks().ToList();
                if (!symbols.Any())
                {
                    return BadRequest("No stock symbols provided and no favorites found.");
                }
            }
            else
            {
                // Use the provided stock symbols directly
                symbols = stockSymbols.Split(',').Select(s => s.Trim()).ToList();
            }

            var stockData = await _stockService.FetchStockDataForMultipleSymbolsAsync(symbols);
            return Ok(stockData);
        }
    }
}
