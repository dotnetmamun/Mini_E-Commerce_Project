using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMart.Models;

namespace MiniMart.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db) => _db = db;

        // list orders with pagination (pageSize = 5)
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 5;
            var query = _db.Orders
                           .AsNoTracking()
                           .Include(o => o.Customer)
                           .Include(o => o.OrderItems)
                               .ThenInclude(oi => oi.Product)
                           .OrderByDescending(o => o.OrderDate);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;

            return View(items);
        }

        // GET: Create order form (choose customer + multiple products)
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = await _db.Customers.AsNoTracking().ToListAsync();
            ViewBag.Products = await _db.Products.AsNoTracking().ToListAsync();

            var model = new OrderCreateDto();
            model.Items.Add(new OrderItemDto()); // ensures at least one row
            return View(model);
        }

        // POST: create order (model binds a DTO)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest();

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    CustomerId = dto.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 0m,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var item in dto.Items)
                {
                    var product = await _db.Products.FindAsync(item.ProductId);
                    if (product == null) throw new Exception($"Product {item.ProductId} not found");

                    if (product.Stock < item.Quantity) throw new Exception($"Not enough stock for {product.Name}");

                    var oi = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.Price
                    };

                    order.OrderItems.Add(oi);

                    product.Stock -= item.Quantity; // update stock
                    order.TotalAmount += oi.Price * oi.Quantity;
                }

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var order = await _db.Orders
                                 .Include(o => o.OrderItems)
                                 .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            ViewBag.Customers = await _db.Customers.AsNoTracking().ToListAsync();
            ViewBag.Products = await _db.Products.AsNoTracking().ToListAsync();

            var dto = new OrderCreateDto
            {
                CustomerId = order.CustomerId,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity
                }).ToList()
            };

            return View(dto);
        }

        // POST: Edit Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest();

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var order = await _db.Orders
                                     .Include(o => o.OrderItems)
                                     .ThenInclude(oi => oi.Product)
                                     .FirstOrDefaultAsync(o => o.Id == id);
                if (order == null) return NotFound();

                // Revert stock for existing items
                foreach (var oldItem in order.OrderItems)
                {
                    var product = await _db.Products.FindAsync(oldItem.ProductId);
                    if (product != null) product.Stock += oldItem.Quantity;
                }

                // Remove old items
                _db.OrderItems.RemoveRange(order.OrderItems);

                // Add new items
                order.OrderItems = new List<OrderItem>();
                order.CustomerId = dto.CustomerId;
                order.TotalAmount = 0m;

                foreach (var item in dto.Items)
                {
                    var product = await _db.Products.FindAsync(item.ProductId);
                    if (product == null) throw new Exception($"Product {item.ProductId} not found");
                    if (product.Stock < item.Quantity) throw new Exception($"Not enough stock for {product.Name}");

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.Price
                    });

                    product.Stock -= item.Quantity;
                    order.TotalAmount += item.Quantity * product.Price;
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // GET: Details Order
        public async Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders
                                 .Include(o => o.Customer)
                                 .Include(o => o.OrderItems)
                                     .ThenInclude(oi => oi.Product)
                                 .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Delete Order
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders
                                 .Include(o => o.Customer)
                                 .Include(o => o.OrderItems)
                                     .ThenInclude(oi => oi.Product)
                                 .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Delete Order
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var order = await _db.Orders
                                     .Include(o => o.OrderItems)
                                     .FirstOrDefaultAsync(o => o.Id == id);
                if (order == null) return NotFound();

                // Return stock
                foreach (var item in order.OrderItems)
                {
                    var product = await _db.Products.FindAsync(item.ProductId);
                    if (product != null) product.Stock += item.Quantity;
                }

                _db.OrderItems.RemoveRange(order.OrderItems);
                _db.Orders.Remove(order);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
