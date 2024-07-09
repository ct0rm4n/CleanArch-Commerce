﻿using Core.Entities.Domain.Blog;
using Core.ViewModel.Catalog;
using Core.Wrappers;

namespace Data.Interfaces
{
    public partial interface IBlogPostService
    {
        BlogPost? Add(BlogPost blogPost);
        List<BlogPost> GetAll();
        bool Update(BlogPost user);
        bool Delete(BlogPost user);
        BlogPost Get(int Id);
        PagedResponse<List<BlogPost>> Search(PaginationFilter filter);
        public IList<string> GetValidation(BlogPostVM viewModel);
        public IEnumerable<BlogPost> BulkInsertReturn(IEnumerable<BlogPost> entitys);
    }
}
