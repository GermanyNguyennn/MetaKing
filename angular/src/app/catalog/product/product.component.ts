import { PagedResultDto } from '@abp/ng.core';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductCategoriesService, ProductCategoryInListDto } from '@proxy/catalog/product-categories';
import { ProductDto, ProductInListDto, ProductsService } from '@proxy/catalog/products';
import { ProductType } from '@proxy/meta-king/products';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Subject, take, takeUntil } from 'rxjs';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { ProductDetailComponent } from './product-detail.component';
import { ProductAttributeComponent } from './product-attribute.component';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
})
export class ProductComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  items: ProductInListDto[] = [];
  public selectedItems: ProductInListDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number;

  productCategories: any[] = [];
  productCategoriesAll: ProductCategoryInListDto[] = [];
  keyword: string = '';
  categoryId: string = '';

  constructor(
    private productService: ProductsService,
    private productCategoryService: ProductCategoriesService,
    private dialogService: DialogService,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit(): void {
    this.loadProductCategories();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.productService
    .getListFilter({
      keyword: this.keyword,
      categoryId: this.categoryId,
      maxResultCount: this.maxResultCount,
      skipCount: this.skipCount,
    })
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: (response: PagedResultDto<ProductInListDto>) => {
        this.items = response.items;
        this.totalCount = response.totalCount;
        if (this.productCategoriesAll && this.productCategoriesAll.length > 0) {
          const dict = new Map<string, ProductCategoryInListDto>();
          this.productCategoriesAll.forEach(c => dict.set(c.id, c));

          this.items.forEach(item => {
            const cat = item.categoryId ? dict.get(item.categoryId) : undefined;
            const parentId = cat ? (cat as any).parentId : undefined;
            if (parentId) {
              const parent = dict.get(parentId);
              (item as any).parentCategoryName = parent ? parent.name : null;
            } else {
              (item as any).parentCategoryName = null;
            }
          });
        }
        this.toggleBlockUI(false);
      },
      error: () => {
        this.toggleBlockUI(false);
      },
    });
  }

  loadProductCategories() {
    this.productCategoryService.getListAll().subscribe((response: ProductCategoryInListDto[]) => {
      this.productCategoriesAll = response;
      response.forEach(element => {
        this.productCategories.push({
          value: element.id,
          label: element.name,
        });
      });

      this.loadData();
    });
  }

  pageChanged(event: any): void {
    this.skipCount = event.page * event.rows;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  showAddModal() {
    const ref = this.dialogService.open(ProductDetailComponent, {
      header: 'Thêm Sản Phẩm',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Thêm Sản Phẩm Thành Công');
        this.selectedItems = [];
      }
    });
  }

  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.notificationService.showError('Bản Phải Chọn 1 Bản Ghi');
      return;
    }
    const id = this.selectedItems[0].id;
    const ref = this.dialogService.open(ProductDetailComponent, {
      data: {
        id: id,
      },
      header: 'Cập Nhật Sản Phẩm',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Cập Nhật Sản Phẩm Thành Công');
        this.selectedItems = [];
      }
    });
  }

  manageProductAttribute(id: string) {
    const ref = this.dialogService.open(ProductAttributeComponent, {
      data: {
        id: id,
      },
      header: 'Quản Lý Thuộc Tính Sản Phẩm',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Cập Nhật Thuộc Tính Sản Phẩm Thành Công');
        this.selectedItems = [];
      }
    });
  }

  deleteItems() {
    if (this.selectedItems.length == 0) {
      this.notificationService.showError('Bản Phải Chọn Ít Nhất 1 Bản Ghi');
      return;
    }
    var ids = [];
    this.selectedItems.forEach(element => {
      ids.push(element.id);
    });
    this.confirmationService.confirm({
      message: 'Bạn Có Muốn Xoá Bản Ghi Này Không?',
      accept: () => {
        this.deleteItemsConfirmed(ids);
      },
    });
  }

  deleteItemsConfirmed(ids: string[]) {
    this.toggleBlockUI(true);
    this.productService
    .deleteMultiple(ids)
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: () => {
        this.loadData();
        this.notificationService.showSuccess('Xóa Bản Ghi Thành Công');
        this.selectedItems = [];
        this.toggleBlockUI(false);
      },
      error: () => {
        this.toggleBlockUI(false);
      },
    });
  }

  getProductTypeName(value: number) {
    return ProductType[value];
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
}