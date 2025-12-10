import { PagedResultDto } from '@abp/ng.core';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ManufacturerDto, ManufacturerInListDto, ManufacturersService } from '@proxy/catalog/manufacturers';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Subject, take, takeUntil } from 'rxjs';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { ManufacturerDetailComponent } from './manufacturer-detail.component';

@Component({
  selector: 'app-manufacturer',
  templateUrl: './manufacturer.component.html',
  styleUrls: ['./manufacturer.component.scss'],
})
export class ManufacturerComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  items: ManufacturerInListDto[] = [];
  public selectedItems: ManufacturerInListDto[] = [];
  rowColors: { [key: string]: string } = {};

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number;

  categoryCategories: any[] = [];
  keyword: string = '';
  categoryId: string = '';

  sortField: string = 'name';
  sortOrder: string = 'ASC';

  constructor(
    private categoryService: ManufacturersService,
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

  sort(field: string) {
    // Nếu click lại cùng field → đảo chiều SORT
    if (this.sortField === field) {
      this.sortOrder = this.sortOrder === 'ASC' ? 'DESC' : 'ASC';
    } else {
      // Click cột mới → reset sortOrder về ASC
      this.sortField = field;
      this.sortOrder = 'ASC';
    }

    // Load lại dữ liệu từ API
    this.loadData();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.categoryService
      .getListFilter({
        keyword: this.keyword,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
        sortField: this.sortField,
        sortOrder: this.sortOrder
      })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PagedResultDto<ManufacturerInListDto>) => {
          this.items = response.items;
          this.totalCount = response.totalCount;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  pageChanged(event: any): void {
    this.skipCount = event.page * event.rows;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  showAddModal() {
    const ref = this.dialogService.open(ManufacturerDetailComponent, {
      header: 'Thêm Nhà Sản Xuất',
      width: '80%',
    });

    ref.onClose.subscribe((data: ManufacturerDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Thêm Nhà Sản Xuất Thành Công');
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
    const ref = this.dialogService.open(ManufacturerDetailComponent, {
      data: {
        id: id,
      },
      header: 'Cập Nhật Nhà Sản Xuất',
      width: '80%',
    });

    ref.onClose.subscribe((data: ManufacturerDto) => {
      if (data) {
        this.loadData();
        this.selectedItems = [];
        this.notificationService.showSuccess('Cập Nhà Sản Xuất Thành Công');
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
        this.notificationService.showSuccess("Xóa Nhà Sản Xuất Thành Công");
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