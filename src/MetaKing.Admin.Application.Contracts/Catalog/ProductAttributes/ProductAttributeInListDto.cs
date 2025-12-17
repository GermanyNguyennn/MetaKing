using System;
using MetaKing.ProductAttributes;
using Volo.Abp.Application.Dtos;

namespace MetaKing.Admin.Catalog.ProductAttributes
{
    public class ProductAttributeInListDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public AttributeType DataType { get; set; }
        public bool IsVisibility { get; set; }
        public bool IsActive { get; set; }
        public bool IsRequired { get; set; }
        public bool IsUnique { get; set; }
        public string? Note { get; set; }
        public Guid Id { get; set; }
    }
}
    