import { PagedResultDto } from '@abp/ng.core';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductAttributeDto, ProductAttributeInListDto, ProductAttributesService } from '@proxy/catalog/product-attributes';
import { AttributeType } from '@proxy/meta-king/product-attributes'; 
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Subject, take, takeUntil } from 'rxjs';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { AttributeDetailComponent } from './attribute-detail.component';
import { MessageConstants } from 'src/app/shared/constants/messages.const';

@Component({
  selector: 'app-attribute',
  templateUrl: './attribute.component.html',
  styleUrls: ['./attribute.component.scss'],
})
export class AttributeComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  items: ProductAttributeInListDto[] = [];
  public selectedItems: ProductAttributeInListDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number;

  AttributeCategories: any[] = [];
  keyword: string = '';
  categoryId: string = '';

  sortField: string = 'name';
  sortOrder: string = 'ASC';

  constructor(
    private attributeService: ProductAttributesService,
    private dialogService: DialogService,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationService,
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
    this.attributeService
    .getListFilter({
      keyword: this.keyword,
      maxResultCount: this.maxResultCount,
      skipCount: this.skipCount,
      sortField: this.sortField,
      sortOrder: this.sortOrder
    })
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: (response: PagedResultDto<ProductAttributeInListDto>) => {
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
    const ref = this.dialogService.open(AttributeDetailComponent, {
      header: 'Thêm Thuộc Tính',
      width: '80%',
    });

    ref.onClose.subscribe((data: ProductAttributeDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess(MessageConstants.CREATED_OK_MSG);
        this.selectedItems = [];
      }
    });
  }

  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.notificationService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    const id = this.selectedItems[0].id;
    const ref = this.dialogService.open(AttributeDetailComponent, {
      data: {
        id: id,
      },
      header: 'Cập Nhât Thuộc Tính',
      width: '80%',
    });

    ref.onClose.subscribe((data: ProductAttributeDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
      }
    });
  }
  
  deleteItems(){
    if(this.selectedItems.length == 0){
      this.notificationService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    var ids =[];
    this.selectedItems.forEach(element=>{
      ids.push(element.id);
    });
    this.confirmationService.confirm({
      message:MessageConstants.CONFIRM_DELETE_MSG,
      accept:()=>{
        this.deleteItemsConfirmed(ids);
      }
    })
  }

  deleteItemsConfirmed(ids: string[]){
    this.toggleBlockUI(true);
    this.attributeService.deleteMultiple(ids).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: ()=>{
        this.loadData();
        this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
        this.selectedItems = [];
        this.toggleBlockUI(false);
      },
      error:()=>{
        this.toggleBlockUI(false);
      }
    })
  }
  
  getAttributeTypeName(value: number){
    return AttributeType[value];
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