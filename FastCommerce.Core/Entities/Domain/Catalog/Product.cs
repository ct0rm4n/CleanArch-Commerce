﻿using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain
{
    public class Product : BaseEntity, IEntity
    {
        public Product()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public string Title { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Html { get; set; }
        public string Seo { get; set; }
        public string Mpn { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
    }

}
