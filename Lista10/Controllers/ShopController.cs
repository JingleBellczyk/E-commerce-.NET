using Lista10.Data;
using Lista10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Lista10.Controllers

{
    public class OrderDetailsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string PaymentMethod { get; set; }
    }
    public class ShopController : Controller
    {
        private readonly MyDbContext _context;

        public ShopController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return NotFound();
            }

            var categories = _context.Category.ToList();
            var articles = _context.Article.ToList();

            //var viewModel = new Tuple<List<Category>, List<Article>>(categories, articles);
            //return View(viewModel);

            ViewBag.SelectedCategory = "All";

            return View(new Tuple<List<Category>, List<Article>>(categories, articles));
        }

        [HttpPost]
        public IActionResult GetArticles(int categoryId)
        {
            var category = _context.Category
                .Include(c => c.Articles)
                .FirstOrDefault(c => c.Id == categoryId);

            if (category != null)
            {
                var viewModel = new Tuple<List<Category>, List<Article>>(_context.Category.ToList(), category.Articles.ToList());

                return View("Index", viewModel);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult AddToCart(int articleId, string returnUrl)
        {
            string existingValue = Request.Cookies[articleId.ToString()];

            // Zwi�ksz warto�� o 1 (lub ustaw na 1, je�li nie istnieje jeszcze ciasteczko)
            int newValue = string.IsNullOrEmpty(existingValue) ? 1 : int.Parse(existingValue) + 1;

            // Ustaw now� warto�� w ciasteczku
            SetCookie(articleId.ToString(), newValue.ToString());

            // Je�li returnUrl nie jest puste, przekieruj u�ytkownika na t� stron�
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Po dodaniu artyku�u do koszyka, mo�esz przekierowa� u�ytkownika gdziekolwiek chcesz
            return RedirectToAction("Index", "Cart");
        }


        public void SetCookie(string key, string value, int? numberOfSeconds = null)
        {
            CookieOptions option = new CookieOptions();
            if (numberOfSeconds.HasValue)
                option.Expires = DateTime.Now.AddSeconds(numberOfSeconds.Value);
            Response.Cookies.Append(key, value, option);
        }

        public IActionResult Cart()
        {
            if (User.IsInRole("Admin"))
            {
                return NotFound();
            }
            // Pobierz zawarto�� koszyka z ciasteczek
            ViewBag.Role = Request.Cookies["Role"];
            List<Tuple<int, int>> cartItems = new List<Tuple<int, int>>();

            foreach (var key in Request.Cookies.Keys)
            {
                if (int.TryParse(key, out int articleId))
                {
                    string quantity = Request.Cookies[key];
                    cartItems.Add(new Tuple<int, int>(articleId, int.Parse(quantity)));
                }
            }

            // Pobierz identyfikatory artyku��w z koszyka
            var articleIds = cartItems.Select(ci => ci.Item1).ToList();

            // Pobierz szczeg�y artyku��w na podstawie identyfikator�w
            var articles = _context.Article.Where(a => articleIds.Contains(a.Id)).ToList();

            // Stw�rz model widoku dla koszyka
            var viewModel = new Tuple<List<Category>, List<Article>, List<Tuple<int, int>>>(_context.Category.ToList(), articles, cartItems);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int articleId)
        {
            if (User.IsInRole("Admin"))
            {
                return NotFound();
            }
            // Usu� ciasteczko z koszyka
            if (Request.Cookies.ContainsKey(articleId.ToString()))
            {
                Response.Cookies.Delete(articleId.ToString());
            }

            // Po usuni�ciu artyku�u z koszyka, przekieruj u�ytkownika z powrotem do koszyka
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(int articleId)
        {
            if (User.IsInRole("Admin"))
            {
                return NotFound();
            }
            // Pobierz warto�� z ciasteczka
            string existingValue = Request.Cookies[articleId.ToString()];

            if (!string.IsNullOrEmpty(existingValue))
            {
                // Zmniejsz warto�� o 1
                int newValue = int.Parse(existingValue) - 1;

                if (newValue > 0)
                {
                    // Ustaw now� warto�� w ciasteczku, je�li warto�� jest wi�ksza ni� 0
                    SetCookie(articleId.ToString(), newValue.ToString());
                }
                else
                {
                    // Usu� ciasteczko, je�li warto�� osi�gn�a 0
                    Response.Cookies.Delete(articleId.ToString());
                }
            }

            // Przekieruj u�ytkownika z powrotem do widoku koszyka
            return RedirectToAction("Cart");
        }

        // POST: Shop/SetRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRole(string Role)
        {
            SetCookie("Role", Role, int.MaxValue); // Set the expiration to a large value or use no expiration.
            Console.WriteLine(Role);
            ViewBag.Role = Role;
            return RedirectToAction("Cart");
        }

        public IActionResult Order()
        {
            if (User.IsInRole("Admin"))
            {
                return NotFound();
            }
            List<Tuple<int, int>> cartItems = new List<Tuple<int, int>>();

            foreach (var key in Request.Cookies.Keys)
            {
                if (int.TryParse(key, out int articleId))
                {
                    string quantity = Request.Cookies[key];
                    cartItems.Add(new Tuple<int, int>(articleId, int.Parse(quantity)));
                }
            }

            // Pobierz identyfikatory artyku��w z koszyka
            var articleIds = cartItems.Select(ci => ci.Item1).ToList();

            // Pobierz szczeg�y artyku��w na podstawie identyfikator�w
            var articles = _context.Article.Where(a => articleIds.Contains(a.Id)).ToList();

            // Pobierz list� kategorii (zmieni�em na List<Category>)
            var categories = _context.Category.ToList();

            // Stw�rz model widoku dla koszyka
            var viewModel = new Tuple<List<Category>, List<Article>, List<Tuple<int, int>>>(categories, articles, cartItems);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ConfirmOrder(OrderDetailsViewModel orderDetails)
        {
            if (User.IsInRole("Admin"))
            {
                return NotFound();
            }
            // Perform order confirmation logic here, e.g., save order details to the database

            // Pass order details to the confirmation view
            foreach (var key in Request.Cookies.Keys)
            {
                if (int.TryParse(key, out _))
                {
                    Response.Cookies.Delete(key);
                }
            }
            ViewBag.OrderDetails = orderDetails;

            // Redirect the user to the confirmation page
            return View("Confirmation");
        }


        [HttpPost]
        public IActionResult EndOrder()
        {
            return RedirectToAction("Cart");
        }

        [Authorize(Policy = "RequireRoleForTurnOnOff")]
        public IActionResult Shutdown()
        {
            return View();
        }
    }
}