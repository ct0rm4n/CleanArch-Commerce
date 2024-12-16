using Core.Entities.Domain.Blog;
using Core.ViewModel.Catalog;
using Data.Commands.Data;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Data.Repositories
{
    public class BlogPostRepository : GenericRepository<BlogPost, BlogPostVM>
    {
        public BlogPostRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
