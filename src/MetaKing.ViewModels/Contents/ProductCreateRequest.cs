using MetaKing.BackendServer.Data;
using MetaKing.ViewModels.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaKing.ViewModels.Contents
{
    public class ProductCreateRequest
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public ProductType ProductType { get; set; }
        public string Slug { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
