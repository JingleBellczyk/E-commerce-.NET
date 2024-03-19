namespace Lista10.Models
{
    public class MemoryRepository: IRepository
    {
        private readonly Dictionary<int, Article> items;
        public MemoryRepository() {
            items = new Dictionary<int, Article>();
            new List<Article>
            {
                new Article{Id = 21, Name = "Art1", Price = 0.0, ExpirationDate = DateTime.Now.AddDays(30)},
                new Article{Id = 22, Name = "Art2", Price = 0.0, ExpirationDate = DateTime.Now.AddDays(30)},
                new Article{Id = 23, Name = "Art3", Price = 0.0, ExpirationDate = DateTime.Now.AddDays(30)},
            }.ForEach(a => AddArticle(a));
        }
        public Article this[int id] => items.ContainsKey(id) ? items[id]:null;
        public IEnumerable<Article> Articles => items.Values;

        public Article AddArticle(Article article)
        {
            if(article.Id == 0)
            {
                int key = items.Count;
                while(items.ContainsKey(key)) { key++; };
                article.Id = key;
            }
            items[article.Id] = article;
            return article;
        }
        public void DeleteArticle(int id) => items.Remove(id);

        public Article UpdateArticle(Article article) => AddArticle(article);

        public Article GetNextArticle(int id)
        {
            return items
                .Select(a => a.Value)
                .Where(a => a.Id < id)
                .OrderBy(a => a.Id)
                .FirstOrDefault();
        }
        public Article GetPreviousArticle(int id)
        {
            return items
                .Select(a => a.Value)
                .Where(a => a.Id < id)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();
        }
    }
}
