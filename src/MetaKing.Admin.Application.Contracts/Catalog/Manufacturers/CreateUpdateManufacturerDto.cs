namespace MetaKing.Admin.Catalog.Manufacturers
{
    public class CreateUpdateManufacturerDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Slug { get; set; }
        public bool IsVisibility { get; set; }
        public bool IsActive { get; set; }
        public string Country { get; set; }
        public string? CoverPictureName { get; set; }
        public string? CoverPictureContent { get; set; }
    }
}
