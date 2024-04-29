using FastCommerce.Core.Entities.Domain.Blog;
using FastCommerce.Core.Wrappers;
using FastCommerce.Data.Commands.Repositories;
using FastCommerce.Data.Interfaces;
using FastCommerce.Service.Helpers;

namespace FastCommerce.Service
{
    public class BlogPostService : IBlogPostService
    {
        public bool Add(BlogPost blogPost)
        {
            bool isAdded = false;
            try
            {
                BlogPostRepository blogPostRepository = new BlogPostRepository();
                isAdded = blogPostRepository.Add(blogPost);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }

        public List<BlogPost> GetAll()
        {
            List<BlogPost> blogPosts = new List<BlogPost>();
            try
            {
                BlogPostRepository blogPostRepository = new BlogPostRepository();
                blogPosts = blogPostRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return blogPosts;
        }

        public BlogPost Get(int Id)
        {
            BlogPost blogPost = new BlogPost();
            try
            {
                BlogPostRepository blogPostRepository = new BlogPostRepository();
                blogPost = blogPostRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return blogPost;
        }

        public bool Update(BlogPost blogPost)
        {
            bool isUpdated = false;
            try
            {
                BlogPostRepository blogPostRepository = new BlogPostRepository();
                isUpdated = blogPostRepository.Update(blogPost);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(BlogPost blogPost)
        {
            bool isDeleted = false;
            try
            {
                BlogPostRepository blogPostRepository = new BlogPostRepository();
                isDeleted = blogPostRepository.Delete(blogPost);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
        public PagedResponse<List<BlogPost>> Search(PaginationFilter filter)
        {

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = new List<BlogPost>();
            if (!string.IsNullOrEmpty(filter.SerachText))
            {
                data = (GetAll()).ToList();
            }
            else
            {
                data = (GetAll()).ToList();
            }
            var pagedData = data
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToList();

            var totalRecords = data.Count();
            var pagedReponse = PaginationHelper.CreatePagedReponse<BlogPost>(pagedData, validFilter, totalRecords, "", "https://localhost:7279");
            return (pagedReponse);
        }
    }
}