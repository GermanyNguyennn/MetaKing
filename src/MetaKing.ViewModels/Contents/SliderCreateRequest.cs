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
    public class SliderCreateRequest
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public StatusType Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
