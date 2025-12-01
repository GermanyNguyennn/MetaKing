 using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MetaKing.ProductCategories
{
    public class ProductCategory : CreationAuditedAggregateRoot<Guid>
    {
        public ProductCategory() {}

        public ProductCategory(Guid id, 
            string name, string code, string slug, 
            int sortOrder, string coverPicture, 
            bool visibility, bool isActive, 
            Guid? parentId, string seoMetaDescription)
        {
            Id = id;
            Name = name;
            Code = code;
            Slug = slug;
            SortOrder = sortOrder;
            CoverPicture = coverPicture;
            Visibility = visibility;
            IsActive = isActive;
            ParentId = parentId;
            SeoMetaDescription = seoMetaDescription;
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Slug { get; set; }
        public int SortOrder { get; set; }
        public string CoverPicture { get; set; }
        public bool Visibility { get; set; }
        public bool IsActive { get; set; }
        public Guid? ParentId { get; set; }
        public string SeoMetaDescription { get; set; }
    }
}
