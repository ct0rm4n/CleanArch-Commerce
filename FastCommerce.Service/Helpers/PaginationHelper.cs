using Core.Wrappers;
using Microsoft.AspNetCore.WebUtilities;

namespace Service.Helpers
{
    public class PaginationHelper
    {

        public static PagedResponse<List<T>> CreatePagedReponse<T>(List<T> pagedData, PaginationFilter validFilter, int totalRecords, string route, string baseUri)
        {
            var respose = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            respose.NextPage =
                validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                ? GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize), route, baseUri)
                : null;
            respose.PreviousPage =
                validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                ? GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize), route, baseUri)
                : null;
            respose.FirstPage = GetPageUri(new PaginationFilter(1, validFilter.PageSize), route, baseUri);
            respose.LastPage = GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize), route, baseUri);
            respose.TotalPages = roundedTotalPages;
            respose.TotalRecords = totalRecords;
            return respose;
        }
        public static Uri GetPageUri(PaginationFilter filter, string route, string baseUri = null)
        {
            var _enpointUri = new Uri(string.Concat(baseUri, route));
            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "pageNumber", filter.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}
