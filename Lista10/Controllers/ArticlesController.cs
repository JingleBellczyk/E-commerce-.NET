using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lista10.Data;
using Lista10.Models;
using Lista10.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Lista10.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly MyDbContext _context;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _he;


        private IRepository Repository { get; set; }

        /*
        public ArticlesController(IRepository repo) => Repository = repo;

        public ViewResult Index() => View(Repository.Articles);
        */
        [HttpPost]
        public IActionResult AddArticle(Article article)
        {
            Repository.AddArticle(article);
            return RedirectToAction("Index");
        }

        public ArticlesController(MyDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment he)
        {
            _context = context;
            _he = he;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Article.Include(a => a.Category);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,ExpirationDate,CategoryId,formFile")] ArticleViewModel articleModel)
        {
            Article article = new Article()
            {
                Id = articleModel.Id,
                Name = articleModel.Name,
                Price = articleModel.Price,
                ExpirationDate = articleModel.ExpirationDate,
                CategoryId = articleModel.CategoryId,
            };
            if (articleModel.formFile != null && articleModel.formFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(articleModel.formFile.FileName);
                var filePath = Path.Combine(_he.WebRootPath, "uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await articleModel.formFile.CopyToAsync(stream);
                }
                article.imagePath = fileName;
            }
            

            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", article.CategoryId);
            return View(articleModel);
        }

        // GET: Articles/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", article.CategoryId);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,ExpirationDate,CategoryId,imagePath")] ArticleViewModel article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(new Article() {
                        Id = article.Id,
                        Name = article.Name,
                        Price = article.Price,
                        ExpirationDate = article.ExpirationDate,
                        CategoryId = article.CategoryId,
                        imagePath = article.imagePath
                        
                    });
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.FindAsync(id);
            if (article != null)
            {
                if (article.imagePath != null)
                {
                    var filePath = Path.Combine(_he.WebRootPath, "uploads", article.imagePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            _context.Article.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
        [Authorize(Policy = "RequireRoleForTurnOnOff")]
        public IActionResult Shutdown()
        {
            return View();
        }
    }
}
