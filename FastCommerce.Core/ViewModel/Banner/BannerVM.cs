using Core.ViewModel.Generic.Abstracts;

namespace Core.ViewModel.Banner
{
    public class BannerVM : IBaseVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Binary { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool Publish { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
