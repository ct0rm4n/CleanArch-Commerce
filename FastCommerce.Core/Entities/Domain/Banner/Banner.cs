using Core.Entities.Abstract;

namespace Core.Entities.Domain.Banner
{
    public class Banner : BaseEntity, IEntity
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Binary { get; set; }
        public bool Publish { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}