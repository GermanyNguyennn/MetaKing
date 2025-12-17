using MetaKing.Manufacturers;
using MetaKing.ProductCategories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace MetaKing.Admin.Catalog.Manufacturers
{
    //[Authorize(MetaKingPermissions.Manufacturer.Default, Policy = "Admin")]
    [AllowAnonymous]
    public class ManufacturersAppService : CrudAppService<
        Manufacturer,
        ManufacturerDto,
        Guid,
        PagedResultRequestDto,
        CreateUpdateManufacturerDto,
        CreateUpdateManufacturerDto>, IManufacturersAppService
    {

        private readonly ManufacturerManager _manufacturerManager;
        private readonly IBlobContainer<ProductCategoryCoverPictureContainer> _fileContainer;
        public ManufacturersAppService(IRepository<Manufacturer, Guid> repository, ManufacturerManager manufacturerManager, IBlobContainer<ProductCategoryCoverPictureContainer> fileContainer)
            : base(repository)
        {

            _manufacturerManager = manufacturerManager;
            _fileContainer = fileContainer;
            //GetPolicyName = MetaKingPermissions.Manufacturer.Default;
            //GetListPolicyName = MetaKingPermissions.Manufacturer.Default;
            //CreatePolicyName = MetaKingPermissions.Manufacturer.Create;
            //UpdatePolicyName = MetaKingPermissions.Manufacturer.Update;
            //DeletePolicyName = MetaKingPermissions.Manufacturer.Delete;

            GetPolicyName = null;
            GetListPolicyName = null;
            CreatePolicyName = null;
            UpdatePolicyName = null;
            DeletePolicyName = null;
        }

        //[Authorize(MetaKingPermissions.Manufacturer.Update)]
        public override async Task<ManufacturerDto> CreateAsync(CreateUpdateManufacturerDto input)
        {
            var manufacturer = await _manufacturerManager.CreateAsync(
                input.Name,
                input.Code,
                input.Slug,
                input.IsVisibility,
                input.IsActive,
                input.Country);

            if (input.CoverPictureContent != null && input.CoverPictureContent.Length > 0)
            {
                await SaveThumbnailImageAsync(input.CoverPictureName!, input.CoverPictureContent);
                manufacturer.CoverPicture = input.CoverPictureName!;
            }

            var result = await Repository.InsertAsync(manufacturer);

            return ObjectMapper.Map<Manufacturer, ManufacturerDto>(result);
        }

        //[Authorize(MetaKingPermissions.Manufacturer.Update)]

        public override async Task<ManufacturerDto> UpdateAsync(Guid id, CreateUpdateManufacturerDto input)
        {
            var manufacturer = await Repository.GetAsync(id);
            if (manufacturer == null)
                throw new BusinessException(MetaKingDomainErrorCodes.ProductIsNotExists);
            manufacturer.Name = input.Name;
            manufacturer.Code = input.Code;
            manufacturer.Slug = input.Slug;
            manufacturer.IsVisibility = input.IsVisibility;
            manufacturer.IsActive = input.IsActive;
            manufacturer.Country = input.Country;

            if (input.CoverPictureContent != null && input.CoverPictureContent.Length > 0)
            {
                await SaveThumbnailImageAsync(input.CoverPictureName!, input.CoverPictureContent);
                manufacturer.CoverPicture = input.CoverPictureName!;
            }

            await Repository.UpdateAsync(manufacturer);

            return ObjectMapper.Map<Manufacturer, ManufacturerDto>(manufacturer);
        }

        //[Authorize(MetaKingPermissions.Manufacturer.Update)]

        private async Task SaveThumbnailImageAsync(string fileName, string base64)
        {
            if (string.IsNullOrWhiteSpace(base64) || string.IsNullOrWhiteSpace(fileName))
                return;

            var commaIndex = base64.IndexOf(',');
            if (commaIndex >= 0)
                base64 = base64.Substring(commaIndex + 1);

            byte[] bytes = Convert.FromBase64String(base64);

            await _fileContainer.SaveAsync(fileName, bytes, overrideExisting: true);
        }


        //[Authorize(MetaKingPermissions.Manufacturer.Default)]

        public async Task<string> GetThumbnailImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null!;
            }
            var thumbnailContent = await _fileContainer.GetAllBytesOrNullAsync(fileName);

            if (thumbnailContent is null)
            {
                return null!;
            }
            var result = Convert.ToBase64String(thumbnailContent);
            return result;
        }

        //[Authorize(MetaKingPermissions.Manufacturer.Delete)]
        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current!.SaveChangesAsync();
        }

        //[Authorize(MetaKingPermissions.Manufacturer.Default)]
        public async Task<List<ManufacturerInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x => x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Manufacturer>, List<ManufacturerInListDto>>(data);

        }

        //[Authorize(MetaKingPermissions.Manufacturer.Default)]
        public async Task<PagedResultDto<ManufacturerInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            // Base query
            var query = await Repository.GetQueryableAsync();

            // Filter
            query = query.WhereIf(
                !string.IsNullOrWhiteSpace(input.Keyword),
                x => x.Name.Contains(input.Keyword!)
            );

            // Chuẩn hoá sort
            var sortField = input.SortField?.ToLower();
            var sortOrder = input.SortOrder?.ToUpper() ?? "ASC";
            bool isAsc = sortOrder == "ASC";

            // Apply sorting
            query = sortField switch
            {
                "id" => isAsc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id),
                "name" => isAsc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                "code" => isAsc ? query.OrderBy(x => x.Code) : query.OrderByDescending(x => x.Code),
                "slug" => isAsc ? query.OrderBy(x => x.Slug) : query.OrderByDescending(x => x.Slug),
                "country" => isAsc ? query.OrderBy(x => x.Country) : query.OrderByDescending(x => x.Country),
                "coverpicture" => isAsc ? query.OrderBy(x => x.CoverPicture) : query.OrderByDescending(x => x.CoverPicture),
                "visibility" => isAsc ? query.OrderBy(x => x.IsVisibility) : query.OrderByDescending(x => x.IsVisibility),
                "isactive" => isAsc ? query.OrderBy(x => x.IsActive) : query.OrderByDescending(x => x.IsActive),
                _ => isAsc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
            };

            // Count
            var totalCount = await AsyncExecuter.LongCountAsync(query);

            // Paging
            var items = await AsyncExecuter.ToListAsync(
                query
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
            );

            return new PagedResultDto<ManufacturerInListDto>(
                totalCount,
                ObjectMapper.Map<List<Manufacturer>, List<ManufacturerInListDto>>(items)
            );
        }
    }
}
