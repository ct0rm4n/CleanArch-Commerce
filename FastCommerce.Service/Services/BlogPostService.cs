using Core.Entities.Domain.Blog;
using Core.ViewModel.Catalog;
using Core.Wrappers;
using Dapper;
using Data.Commands.Repositories;
using Data.Interfaces;
using Service.Helpers;

namespace Service
{
    public partial class BlogPostService : IBlogPostService
    {
        private readonly BlogPostRepository _repository;

        public BlogPostService(BlogPostRepository repository)
        { 
            _repository = repository;
        }

        public BlogPost? Add(BlogPost blogPost)
        {
            var isAdded = false;
            try
            {
                var inserted = _repository.AddReturn(blogPost);
                isAdded = inserted.Item2;
                return inserted.Item1;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        public List<BlogPost> GetAll()
        {
            List<BlogPost> blogPosts = new List<BlogPost>();
            try
            {
                blogPosts = _repository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return blogPosts;
        }

        public (List<BlogPost>, int) GetPagged(string text, int page_size= 20, int page = 1)
        {
            IEnumerable<BlogPost> blogPosts = new List<BlogPost>();
            int total = page; 
            try
            {
                var entity = new List<string>() { "Name", "Url", "Html"};
                (blogPosts, page) = _repository.GetPagged(text, entity, page_size, page);
            }
            catch (Exception ex)
            {
            }
            return (blogPosts.AsList<BlogPost>(), page);
        }


        public BlogPost Get(int Id)
        {
            BlogPost blogPost = new BlogPost();
            try
            {
                blogPost = _repository.GetById(Id);
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
                isUpdated = _repository.Update(blogPost);
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
                isDeleted = _repository.Delete(blogPost);
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
            var totalRecords = 0;

            (data, totalRecords) = GetPagged(filter.SerachText, validFilter.PageSize, validFilter.PageNumber);
            var pagedReponse = PaginationHelper.CreatePagedReponse<BlogPost>(data, validFilter, totalRecords, "", "https://localhost:7152/BlogPost/getall");
            return (pagedReponse);
        }
        public IList<string> GetValidation(BlogPostVM viewModel)
        {
            return _repository.GetValidationMessage(viewModel);
        }

        public IEnumerable<BlogPost> BulkInsertReturn(IEnumerable<BlogPost> entitys)
        {
            return _repository.BulkInsertReturn(entitys);
        }
    }
}