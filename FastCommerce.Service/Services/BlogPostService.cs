using Core.Entities.Domain.Blog;
using Core.ViewModel.Catalog;
using Core.Wrappers;
using Data.Commands.Repositories;
using Data.Interfaces;
using Service.Helpers;
using System.Buffers;
using System.ComponentModel.DataAnnotations;

namespace Service
{
    public partial class BlogPostService : IBlogPostService
    {
        private readonly BlogPostRepository _repository = new BlogPostRepository();
                
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

        
        public IList<string> GetValidationMessage(BlogPostVM viewModel)
        {
            try
            {
                var validationContext = new ValidationContext(viewModel, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);
                var messages = new List<string>();
                // If there any exception return them in the return result
                if (!isValid)
                {
                    OperationStatus opStatus = new OperationStatus();

                    foreach (ValidationResult message in validationResults)
                    {
                        messages.Add(message.ErrorMessage);
                    }

                    return messages;
                }
                return messages;
            }
            catch(Exception ex)
            {
                return new List<string>() { $"Error: {ex.Message}" };
            }

        }
    }
}