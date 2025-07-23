using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MetaKing.BackendServer.Data.Interfaces;
using MetaKing.BackendServer.Data.Entities;
using MetaKing.ViewModels.Enum;

namespace MetaKing.BackendServer.Data
{

    public class SliderModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }
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
