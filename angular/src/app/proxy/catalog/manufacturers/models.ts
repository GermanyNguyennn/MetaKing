import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateManufacturerDto {
  name?: string;
  code?: string;
  slug?: string;
  isVisibility: boolean;
  isActive: boolean;
  country?: string;
  coverPictureName?: string;
  coverPictureContent?: string;
}

export interface ManufacturerDto {
  name?: string;
  code?: string;
  slug?: string;
  country?: string;
  coverPicture?: string;
  isVisibility: boolean;
  isActive: boolean;
  id?: string;
}

export interface ManufacturerInListDto extends EntityDto<string> {
  name?: string;
  code?: string;
  slug?: string;
  country?: string;
  coverPicture?: string;
  isVisibility: boolean;
  isActive: boolean;
}
