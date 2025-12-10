using System;
using Volo.Abp.Application.Dtos;

namespace MetaKing.Admin.Catalog.Manufacturers
{
    public class ManufacturerInListDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Slug { get; set; }
        public string Country { get; set; }
        public string CoverPicture { get; set; }
        public bool IsVisibility { get; set; }
        public bool IsActive { get; set; }
    }
}
