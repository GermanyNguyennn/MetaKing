using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaKing.ProductCategories;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace MetaKing.Manufacturers
{
    public class ManufacturerManager : DomainService
    {
        private readonly IRepository<Manufacturer, Guid> _manufacturerRepository;
        public ManufacturerManager(IRepository<Manufacturer, Guid> manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<Manufacturer> CreateAsync(string name, string code, string slug,
            bool visibility,
            bool isActive, string country)
        {
            if (await _manufacturerRepository.AnyAsync(x => x.Name == name))
                throw new UserFriendlyException("Tên nhà sản xuất đã tồn tại", MetaKingDomainErrorCodes.ProductNameAlreadyExists);
            if (await _manufacturerRepository.AnyAsync(x => x.Code == code))
                throw new UserFriendlyException("Mã nhà sản xuất đã tồn tại", MetaKingDomainErrorCodes.ProductCodeAlreadyExists);           

            return new Manufacturer(Guid.NewGuid(), name, code, slug, string.Empty, visibility, isActive, country);
        }
    }
}
