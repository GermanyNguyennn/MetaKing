import { ConfirmationService } from 'primeng/api';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ProductAttributeInListDto, ProductAttributesService } from '@proxy/catalog/product-attributes';
import { ProductsService } from '@proxy/catalog/products';
import { ProductDto } from '@proxy/catalog/products';
import { ProductAttributeValueDto } from '@proxy/catalog/products/attributes'; 
import { AttributeType } from '@proxy/meta-king/product-attributes'; 
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { NotificationService } from 'src/app/shared/services/notification.service'; 
import { UtilityService } from 'src/app/shared/services/utility.service'; 
import { MessageConstants } from 'src/app/shared/constants/messages.const';

@Component({
  selector: 'app-product-attribute',
  templateUrl: './product-attribute.component.html',
})
export class ProductAttributeComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  btnDisabled = false;
  public form: FormGroup;

  attributes: any[] = [];
  fullAttributes: any[] = [];
  productAttributes: any[] = [];
  showDateTimeControl: boolean = false;
  showDecimalControl: boolean = false;
  showIntControl: boolean = false;
  showVarcharControl: boolean = false;
  showTextControl: boolean = false;

  constructor(
    private productAttributeService: ProductAttributesService,
    private productService: ProductsService,
    private fb: FormBuilder,
    private config: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit(): void {
    this.buildForm();
    this.initFormData();
  }

  initFormData() {
    const attributes$ = this.productAttributeService.getListAll();

    this.toggleBlockUI(true);

    forkJoin({ attributes: attributes$ })
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: (response) => {
        const attributes = response.attributes as ProductAttributeInListDto[];

        this.fullAttributes = attributes;

        this.attributes = attributes.map(x => ({
          value: x.id,
          label: x.name,
        }));

        this.loadFormDetails(this.config.data?.id);
      },
      error: () => {
        this.toggleBlockUI(false);
      },
    });
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.productService
      .getListProductAttributeAll(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: ProductAttributeValueDto[]) => {
          this.productAttributes = response;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  saveChanged() {
    this.toggleBlockUI(true);
    this.productService
      .addProductAttribute(this.form.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.toggleBlockUI(false);
          this.loadFormDetails(this.config.data.id);
        },
        error: err => {
          this.notificationService.showError(err.error.error.message);
          this.toggleBlockUI(false);
        },
      });
  }

  private buildForm() {
    this.form = this.fb.group({
      productId: new FormControl(this.config.data.id),
      attributeId: new FormControl(null, Validators.required),
      dateTimeValue: new FormControl(null),
      decimalValue: new FormControl(null),
      intValue: new FormControl(null),
      varcharValue: new FormControl(null),
      textValue: new FormControl(null),
    });
  }
  
  removeItem(attribute: ProductAttributeValueDto) {
    var id = '';
    if (attribute.dataType == AttributeType.Date) {
      id = attribute.dateTimeId;
    } else if (attribute.dataType == AttributeType.Decimal) {
      id = attribute.decimalId;
    } else if (attribute.dataType == AttributeType.Int) {
      id = attribute.intId;
    } else if (attribute.dataType == AttributeType.Text) {
      id = attribute.textId;
    } else if (attribute.dataType == AttributeType.Varchar) {
      id = attribute.varcharId;
    }
    this.confirmationService.confirm({
      message: MessageConstants.CONFIRM_DELETE_MSG,
      accept: () => {
        this.deleteItemsConfirmed(attribute, id);
      },
    });
  }

  deleteItemsConfirmed(attribute: ProductAttributeValueDto, id: string) {
    this.toggleBlockUI(true);
    this.productService
      .removeProductAttribute(attribute.attributeId, id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
          this.loadFormDetails(this.config.data?.id);
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  selectAttribute(event: any) {
    var dataType = this.fullAttributes.filter(x => x.id == event.value)[0].dataType;
    this.showDateTimeControl = false;
    this.showDecimalControl = false;
    this.showIntControl = false;
    this.showTextControl = false;
    this.showVarcharControl = false;
    if (dataType == AttributeType.Date) {
      this.showDateTimeControl = true;
    } else if (dataType == AttributeType.Decimal) {
      this.showDecimalControl = true;
    } else if (dataType == AttributeType.Int) {
      this.showIntControl = true;
    } else if (dataType == AttributeType.Text) {
      this.showTextControl = true;
    } else if (dataType == AttributeType.Varchar) {
      this.showVarcharControl = true;
    }
  }

  getDataTypeName(value: number) {
    return AttributeType[value];
  }

  getValueByType(attribute: ProductAttributeValueDto, value: number) {
    if (attribute.dataType == AttributeType.Date) {
      return attribute.dateTimeValue;
    } else if (attribute.dataType == AttributeType.Decimal) {
      return attribute.decimalValue;
    } else if (attribute.dataType == AttributeType.Int) {
      return attribute.intValue;
    } else if (attribute.dataType == AttributeType.Text) {
      return attribute.textValue;
    } else if (attribute.dataType == AttributeType.Varchar) {
      return attribute.varcharValue;
    }
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
      this.btnDisabled = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
        this.btnDisabled = false;
      }, 1000);
    }
  }
}