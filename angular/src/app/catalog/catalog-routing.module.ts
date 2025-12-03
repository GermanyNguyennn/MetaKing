import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProductComponent } from './product/product.component';
import { AttributeComponent } from './attribute/attribute.component';
import { PermissionGuard } from '@abp/ng.core';
import { CategoryComponent } from './category/category.component';
import { ManufacturerComponent } from './manufacturer/manufacturer.component';

const routes: Routes = [
  { 
    path: 'product', component: ProductComponent,
    canActivate: [PermissionGuard],
    // data: {
    //   requiredPolicy: 'MetaKingAdminCatalog.Product',
    // }, 
    
  },
  { 
    path: 'attribute', component: AttributeComponent,
    canActivate: [PermissionGuard],
    // data: {
    //   requiredPolicy: 'MetaKingAdminCatalog.Attribute',
    // }, 
  },
  { 
    path: 'category', component: CategoryComponent,
    canActivate: [PermissionGuard],
    // data: {
    //   requiredPolicy: 'MetaKingAdminCatalog.Category',
    // }, 
  },
  { 
    path: 'manufacturer', component: ManufacturerComponent,
    canActivate: [PermissionGuard],
    // data: {
    //   requiredPolicy: 'MetaKingAdminCatalog.Component',
    // }, 
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CatalogRoutingModule {}
