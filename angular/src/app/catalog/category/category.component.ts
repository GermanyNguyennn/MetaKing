import { PagedResultDto } from '@abp/ng.core';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductCategoryDto, ProductCategoryInListDto, ProductCategoriesService } from '@proxy/catalog/product-categories';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Subject, take, takeUntil } from 'rxjs';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { CategoryDetailComponent } from './category-detail.component';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.scss'],
})
export class CategoryComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  items: ProductCategoryInListDto[] = [];
  public selectedItems: ProductCategoryInListDto[] = [];
  rowColors: { [key: string]: string } = {};


  //Paging variables
  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number;

  //Filter
  categoryCategories: any[] = [];
  keyword: string = '';
  categoryId: string = '';

  constructor(
    private categoryService: ProductCategoriesService,
    private dialogService: DialogService,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit(): void {
    this.loadData();
  }

  // Hàm tạo màu HSL dựa trên số lượng category
  private generateColors(count: number): string[] {
    const colors: string[] = [];
    for (let i = 0; i < count; i++) {
      const hue = (i * 360) / count;
      colors.push(`hsl(${hue}, 70%, 85%)`);
    }
    return colors;
  }

  private assignColors() {
    // Lấy danh sách parentId duy nhất
    const parentIds = Array.from(new Set(this.items.map(x => x.parentId || x.id)));
    const colors = this.generateColors(parentIds.length);

    parentIds.forEach((id, index) => {
      this.rowColors[id] = colors[index];
    });
  }

  // Lấy màu cho từng dòng
  getRowColor(row: ProductCategoryInListDto): string {
    const parentKey = row.parentId || row.id;
    return this.rowColors[parentKey] || 'transparent';
  }

  loadData() {
    this.toggleBlockUI(true);
    this.categoryService
      .getListFilter({
        keyword: this.keyword,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PagedResultDto<ProductCategoryInListDto>) => {
          this.items = response.items;
          this.totalCount = response.totalCount;
          this.assignColors();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  pageChanged(event: any): void {
    this.skipCount = event.page * event.rows; // không trừ 1
    this.maxResultCount = event.rows;
    this.loadData();
  }

  showAddModal() {
    const ref = this.dialogService.open(CategoryDetailComponent, {
      header: 'Thêm Danh Mục',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductCategoryDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Thêm Danh Mục Thành Công');
        this.selectedItems = [];
      }
    });
  }

  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.notificationService.showError('Bạn Phải Chọn Một Bản Ghi');
      return;
    }
    const id = this.selectedItems[0].id;
    const ref = this.dialogService.open(CategoryDetailComponent, {
      data: {
        id: id,
      },
      header: 'Cập Nhật Danh Mục',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductCategoryDto) => {
      if (data) {
        this.loadData();
        this.selectedItems = [];
        this.notificationService.showSuccess('Cập Nhật Danh Mục Thành Công');
      }
    });
  }
  deleteItems(){
    if(this.selectedItems.length == 0){
      this.notificationService.showError("Bạn Phải Chọn Ít Nhất Một Bản Ghi");
      return;
    }
    var ids =[];
    this.selectedItems.forEach(element=>{
      ids.push(element.id);
    });
    this.confirmationService.confirm({
      message:'Bạn Có Muốn Xoá Bản Ghi Này Không?',
      accept:()=>{
        this.deleteItemsConfirmed(ids);
      }
    })
  }

  deleteItemsConfirmed(ids: string[]){
    this.toggleBlockUI(true);
    this.categoryService.deleteMultiple(ids).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: ()=>{
        this.notificationService.showSuccess("Xóa Danh Mục Thành Công");
        this.loadData();
        this.selectedItems = [];
        this.toggleBlockUI(false);
      },
      error:()=>{
        this.toggleBlockUI(false);
      }
    })
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