using FastCommerce.Core.Entities.Domain;
using FastCommerce.Core.Entities.Domain.Blog;
using FastCommerce.Core.Wrappers;

namespace FastCommerce.Data.Interfaces
{
    public interface IBlogPostService
    {
        bool Add(BlogPost user);
        List<BlogPost> GetAll();
        bool Update(BlogPost user);
        bool Delete(BlogPost user);
        BlogPost Get(int Id);
        PagedResponse<List<BlogPost>> Search(PaginationFilter filter);
    }
}
