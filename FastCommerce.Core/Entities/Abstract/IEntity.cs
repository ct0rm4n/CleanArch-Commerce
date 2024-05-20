

using Core.Entities.Enum;

namespace Core.Entities.Abstract
{
    internal interface IEntity
    {
        public DateTime? InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public StatusEntity Status { get; set; }
    }
}
