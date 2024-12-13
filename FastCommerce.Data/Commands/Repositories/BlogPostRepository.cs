using Core.Entities.Domain.Blog;
using Core.ViewModel.Catalog;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Repositories
{
    public class BlogPostRepository : GenericRepository<BlogPost, BlogPostVM>
    {
        public BlogPostRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
