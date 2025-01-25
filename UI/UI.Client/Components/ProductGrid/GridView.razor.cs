using BlazorBootstrap;
using Core.Entities.Domain;
using Core.ViewModel.Catalog;
using Core.Wrappers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Json;

namespace UI.Client.Components.ProductGrid
{
    public partial class GridView
    {
        [Inject]
        IConfiguration Configuration { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }


        [Inject]
        public IJSRuntime js { get; set; }

        public HttpClient _client = new HttpClient();
        public List<Product> listProduct { get; set; }
        int PageIndex = 0;
        int TotalPages = 1;
        public string token_jwt { get; set; }
        protected override async Task OnInitializedAsync()
        {
            
            var respose = new PagedResponse<List<Product>>(null, 0, int.MaxValue);
            if (listProduct is null)
            {
                PageIndex = 1;
                var path = $"{Configuration["Api.Gateway"]}catalog-gate/Catalog/product/Getall?PageSize=6";
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
            var respose = await _client.GetFromJsonAsync<PagedResponse<List<Product>>>($"{Configuration["Api.Gateway"]}catalog-gate/Catalog/product/Getall?PageNumber={page}&PageSize=6");
            listProduct = respose.Data;
            StateHasChanged();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

        private int Quantidade = 0;
        public string token { get; set; } = string.Empty;

        private void Decrementar(int productId)
        {
            if (Quantidade > 1)
            {
                Quantidade--;
            }
            
            ChangeCart(Quantidade, productId);
        }

        private void Incrementar(int productId)
        {
            Quantidade++;
            ChangeCart(Quantidade, productId);
        }

        private void ShowMessage(string toastType, string title, string description)
        {         

            js.InvokeVoidAsync("ToastShow", new object[] { toastType, title, description });
        }
        public async Task ChangeCart(int qtd, int productId)
        {
            try
            {
                _client = new HttpClient();
                token_jwt = await GetFromSessionStorage("access_token");
                var path = $"{Configuration["Api.Gateway"]}checkout-gate/Checkout/add/{productId}?qtd={qtd}";
                if (string.IsNullOrEmpty(token_jwt))
                {
                    NavigationManager.NavigateTo($"/login");
                    return;
                }

                _client.DefaultRequestHeaders.Add("Authorization", token_jwt);
                var base_request = _client.PostAsync(path, null).Result;
                if (base_request.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ShowMessage("error", "Não autorizado", "Você precisa estar logado para realizar essa ação");
                    await Task.Delay(3000);
                    NavigationManager.NavigateTo($"/login");
                    return;
                }
                else{
                    ShowMessage("success", "Carrinho adicionado com sucesso", "O produto foi adicionado com sucesso ao carrinho de compra");
                }                
            }
            catch(Exception ex)
            {

            }
        }
        private async Task SaveToSessionStorage(string key, string value)
        {
            await js.InvokeVoidAsync("sessionStorage.setItem", key, value);
        }

        private async Task<string> GetFromSessionStorage(string key)
        {
            try
            {
                return await js.InvokeAsync<string>("sessionStorage.getItem", key);
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        private async Task RemoveFromSessionStorage(string key)
        {
            await js.InvokeVoidAsync("sessionStorage.removeItem", key);
        }


        private async Task RetrieveToken()
        {
            token = await GetFromSessionStorage("access_token");
        }

        private async Task ClearToken()
        {
            await RemoveFromSessionStorage("access_token");
        }
    }
}
