namespace Core.ViewModel.Generic.Abstracts
{
    public class SettingsVM : IBaseVM
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}
