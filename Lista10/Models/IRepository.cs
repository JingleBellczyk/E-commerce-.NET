namespace Lista10.Models
{
    public interface IRepository
    {
        IEnumerable<Article>  Articles { get; } 
        Article this[int id] { get; }
        Article AddArticle(Article article);
        Article UpdateArticle(Article article); 
        void DeleteArticle(int id);
        Article GetNextArticle(int id);
        Article GetPreviousArticle(int id);
    }
}
