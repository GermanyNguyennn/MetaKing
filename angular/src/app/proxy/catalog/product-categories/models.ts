import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateProductCategoryDto {
  name?: string;
  code?: string;
  slug?: string;
  isVisibility: boolean;
  isActive: boolean;
  parentId?: string;
  coverPictureName?: string;
  coverPictureContent?: string;
}

export interface ProductCategoryDto {
  name?: string;
  code?: string;
  slug?: string;
  coverPicture?: string;
  isVisibility: boolean;
  isActive: boolean;
  parentId?: string;
  parentName?: string;
  id?: string;
}

export interface ProductCategoryInListDto extends EntityDto<string> {
  name?: string;
  code?: string;
  slug?: string;
  coverPicture?: string;
  isVisibility: boolean;
  isActive: boolean;
  parentId?: string;
  parentName?: string;
}
