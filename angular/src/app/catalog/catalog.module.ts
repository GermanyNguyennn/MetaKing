import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module'; 
import { CatalogRoutingModule } from './catalog-routing.module'; 
import { ProductComponent} from './product/product.component';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { CalendarModule } from 'primeng/calendar';
import { BadgeModule } from 'primeng/badge';
import { ImageModule } from 'primeng/image';
import { EditorModule } from 'primeng/editor';
import { ProductDetailComponent } from './product/product-detail.component';
import { MetaKingSharedModule } from 'src/app/shared/modules/metaking-shared.module'; 
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProductAttributeComponent } from './product/product-attribute.component';
import { AttributeDetailComponent } from './attribute/attribute-detail.component';
import { AttributeComponent } from './attribute/attribute.component';


@NgModule({
  declarations: [ProductComponent, ProductDetailComponent, ProductAttributeComponent, AttributeComponent, AttributeDetailComponent],
  imports: [SharedModule, CatalogRoutingModule, PanelModule, TableModule, PaginatorModule, BlockUIModule, ProgressSpinnerModule, ButtonModule, DropdownModule, CheckboxModule, InputTextModule, InputTextareaModule,
    InputNumberModule, DynamicDialogModule, ConfirmDialogModule, CalendarModule, BadgeModule, ImageModule, EditorModule, MetaKingSharedModule, FormsModule, ReactiveFormsModule],
    entryComponents:[
      ProductDetailComponent, ProductAttributeComponent, AttributeDetailComponent
    ]
})
export class CatalogModule {}
