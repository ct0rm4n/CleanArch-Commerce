using Core.Entities.Domain.Banner;
using Core.Wrappers;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace UI.Client.Components
{
    public partial class BannerSlider
    {
        [Inject]
        IConfiguration Configuration { get; set; }
        private readonly HttpClient _client = new HttpClient();
        public List<Banner> listBanner { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var respose = new PagedResponse<List<Banner>>(null, 0, int.MaxValue);
            if (listBanner is null)
            {
                _client.DefaultRequestHeaders.Add("Authorization", Configuration["SecretKeys.ApiKey"]);
                respose = await _client.GetFromJsonAsync<PagedResponse<List<Banner>>>($"{Configuration["Api.Catalog"]}banner/getall?PageSize=" + int.MaxValue);
                listBanner = respose.Data;
            }
            await base.OnInitializedAsync();
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }
    }
}
