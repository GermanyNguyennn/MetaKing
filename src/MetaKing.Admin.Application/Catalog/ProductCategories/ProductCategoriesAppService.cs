using MetaKing.Admin.Catalog.Products;
using MetaKing.Admin.Permissions;
using MetaKing.ProductCategories;
using MetaKing.Products;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace MetaKing.Admin.Catalog.ProductCategories
{
    //[Authorize(MetaKingPermissions.ProductCategory.Default, Policy = "Admin")]
    [AllowAnonymous]
    public class ProductCategoriesAppService : CrudAppService<
        ProductCategory,
        ProductCategoryDto,
        Guid,
        PagedResultRequestDto,
        CreateUpdateProductCategoryDto,
        CreateUpdateProductCategoryDto>, IProductCategoriesAppService
    {
        private readonly ProductCategoryManager _productCategoryManager;
        private readonly IBlobContainer<ProductCategoryCoverPictureContainer> _fileContainer;

        public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository, ProductCategoryManager productCategoryManager, IBlobContainer<ProductCategoryCoverPictureContainer> fileContainer)
            : base(repository)
        {
            _productCategoryManager = productCategoryManager;
            _fileContainer = fileContainer;

            //GetPolicyName = MetaKingPermissions.ProductCategory.Default;
            //GetListPolicyName = MetaKingPermissions.ProductCategory.Default;
            //CreatePolicyName = MetaKingPermissions.ProductCategory.Create;
            //UpdatePolicyName = MetaKingPermissions.ProductCategory.Update;
            //DeletePolicyName = MetaKingPermissions.ProductCategory.Delete;

            GetPolicyName = null;
            GetListPolicyName = null;
            CreatePolicyName = null;
            UpdatePolicyName = null;
            DeletePolicyName = null;
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Update)]
        public override async Task<ProductCategoryDto> CreateAsync(CreateUpdateProductCategoryDto input)
        {
            var productCategory = await _productCategoryManager.CreateAsync(
                input.Name,
                input.Code,
                input.Slug,
                input.SortOrder,
                input.Visibility,
                input.IsActive,
                input.ParentId,
                input.SeoMetaDescription);

            if (input.CoverPictureContent != null && input.CoverPictureContent.Length > 0)
            {
                await SaveThumbnailImageAsync(input.CoverPictureName, input.CoverPictureContent);
                productCategory.CoverPicture = input.CoverPictureName;
            }

            var result = await Repository.InsertAsync(productCategory);

            return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(result);
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Update)]

        public override async Task<ProductCategoryDto> UpdateAsync(Guid id, CreateUpdateProductCategoryDto input)
        {
            var productCategory = await Repository.GetAsync(id);
            if (productCategory == null)
                throw new BusinessException(MetaKingDomainErrorCodes.ProductIsNotExists);
            productCategory.Name = input.Name;
            productCategory.Code = input.Code;
            productCategory.Slug = input.Slug;
            productCategory.SortOrder = input.SortOrder;
            productCategory.Visibility = input.Visibility;
            productCategory.IsActive = input.IsActive;
            productCategory.SeoMetaDescription = input.SeoMetaDescription;

            if (input.CoverPictureContent != null && input.CoverPictureContent.Length > 0)
            {
                await SaveThumbnailImageAsync(input.CoverPictureName, input.CoverPictureContent);
                productCategory.CoverPicture = input.CoverPictureName;
            }

            await Repository.UpdateAsync(productCategory);

            return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(productCategory);
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Update)]

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


        //[Authorize(MetaKingPermissions.ProductCategory.Default)]

        public async Task<string> GetThumbnailImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            var thumbnailContent = await _fileContainer.GetAllBytesOrNullAsync(fileName);

            if (thumbnailContent is null)
            {
                return null;
            }
            var result = Convert.ToBase64String(thumbnailContent);
            return result;
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Delete)]

        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Default)]

        public async Task<List<ProductCategoryInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x=>x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);

        }

        //[Authorize(MetaKingPermissions.ProductCategory.Default)]
        public async Task<List<ProductCategoryInListDto>> GetListParentAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x => x.ParentId == null && x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Default)]
        public async Task<List<ProductCategoryInListDto>> GetListChildAsync(Guid parentId)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x => x.ParentId == parentId && x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Default)]
        public async Task<PagedResultDto<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ProductCategoryInListDto>(totalCount,ObjectMapper.Map<List<ProductCategory>,List<ProductCategoryInListDto>>(data));
        }
    }
}
