using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniMart.Models;
using System.Diagnostics;

namespace MiniMart.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;
        public ReportsController(IConfiguration config, ApplicationDbContext db)
        {
            _config = config;
            _db = db;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> TopSellingProducts()
        {
            var connString = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connString);
            await conn.OpenAsync();

            var swEf = Stopwatch.StartNew();
            // EF query for comparison (aggregate)
            var efResult = await _db.OrderItems
                .AsNoTracking()
                .GroupBy(oi => oi.ProductId)
                .Select(g => new {
                    ProductId = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    Sales = g.Sum(x => x.Quantity * x.Price)
                })
                .Join(_db.Products, a => a.ProductId, p => p.Id, (a, p) => new { p.Name, a.Quantity, a.Sales })
                .OrderByDescending(x => x.Quantity)
                .ToListAsync();
            swEf.Stop();

            var swDapper = Stopwatch.StartNew();
            var sql = @"
            SELECT p.Name, SUM(oi.Quantity) AS TotalQuantity, SUM(oi.Quantity * oi.Price) AS TotalSales
            FROM OrderItems oi
            INNER JOIN Products p ON oi.ProductId = p.Id
            GROUP BY p.Name
            ORDER BY TotalQuantity DESC;
        ";

            var dapperResult = await conn.QueryAsync<TopSellingProductDto>(sql);
            swDapper.Stop();

            Console.WriteLine($"EF time ms: {swEf.ElapsedMilliseconds}, Dapper time ms: {swDapper.ElapsedMilliseconds}");

            var vm = new TopSellingReportVm
            {
                EfElapsedMs = swEf.ElapsedMilliseconds,
                DapperElapsedMs = swDapper.ElapsedMilliseconds,
                DapperResults = dapperResult.ToList()
            };

            return View(vm);
        }
    }
}
