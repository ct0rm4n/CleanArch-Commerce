using Core.Entities.Domain;
using Core.ViewModel.Catalog;
using Core.Wrappers;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace UI.Client.Components.ProductGrid
{
    public partial class GridView
    {
        [Inject]
        IConfiguration Configuration { get; set; }
        private readonly HttpClient _client = new HttpClient();
        public List<Product> listProduct { get; set; }
        int PageIndex = 0;
        int TotalPages = 1;

        protected override async Task OnInitializedAsync()
        {
            var respose = new PagedResponse<List<Product>>(null, 0, int.MaxValue);
            if (listProduct is null)
            {
                PageIndex = 1;
                var path = $"{Configuration["Api.Catalog"]}product/Getall?PageSize=6";
                _client.DefaultRequestHeaders.Add("Authorization", Configuration["SecretKeys.ApiKey"]);
                respose = await _client.GetFromJsonAsync<PagedResponse<List<Product>>>(path);
                listProduct = respose.Data;
                TotalPages = respose.TotalPages;
            }
            await base.OnInitializedAsync();
        }
        public async Task SelectedPage(int index)
        {
            PageIndex = index + 1;
            await OpenPage(index);
        }
        private async Task OpenPage(int page)
        {
            PageIndex = page;
            var respose = await _client.GetFromJsonAsync<PagedResponse<List<Product>>>($"{Configuration["Api.Catalog"]}product/Getall?PageNumber={page}&PageSize=6");
            listProduct = respose.Data;
            StateHasChanged();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }


    }
}
