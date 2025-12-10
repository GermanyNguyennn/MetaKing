import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { ProductAttributeDto, ProductAttributesService } from '@proxy/catalog/product-attributes';
import { attributeTypeOptions } from '@proxy/meta-king/product-attributes';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { UtilityService } from 'src/app/shared/services/utility.service'; 

@Component({
  selector: 'app-attribute-detail',
  templateUrl: './attribute-detail.component.html',
})
export class AttributeDetailComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  btnDisabled = false;
  public form: FormGroup;

  dataTypes: any[] = [];
  selectedEntity = {} as ProductAttributeDto;

  constructor(
    private attributeService: ProductAttributesService,
    private fb: FormBuilder,
    private config: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private utilService: UtilityService,
    private notificationService: NotificationService
  ) {}

  validationMessages = {
    
  };

  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit(): void {
    this.buildForm();
    this.loadAttributeTypes();
    this.initFormData();
  }

  initFormData() {
    if (this.utilService.isEmpty(this.config.data?.id) == true) {
      this.toggleBlockUI(false);
    } else {
      this.loadFormDetails(this.config.data?.id);
    }
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.attributeService
    .get(id)
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: (response: ProductAttributeDto) => {
        this.selectedEntity = response;
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
    if (this.utilService.isEmpty(this.config.data?.id) == true) {
      this.attributeService
        .create(this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.toggleBlockUI(false);
            this.ref.close(this.form.value);
          },
          error: err => {
            this.notificationService.showError(err.error.error.message);
            this.toggleBlockUI(false);
          },
        });
    } else {
      this.attributeService
      .update(this.config.data?.id, this.form.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.toggleBlockUI(false);
          this.ref.close(this.form.value);
        },
        error: err => {
          this.notificationService.showError(err.error.error.message);
          this.toggleBlockUI(false);
        },
      });
    }
  }

  loadAttributeTypes() {
    attributeTypeOptions.forEach(element => {
      this.dataTypes.push({
        value: element.value,
        label: element.key,
      });
    });
  }

  private buildForm() {
    this.form = this.fb.group({
      label: new FormControl(
        this.selectedEntity.label || null,
        Validators.compose([Validators.required, Validators.maxLength(250)])
      ),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      dataType: new FormControl(this.selectedEntity.dataType || null, Validators.required),
      isVisibility: new FormControl(this.selectedEntity.isVisibility || true),
      isActive: new FormControl(this.selectedEntity.isActive || true),
      isRequired: new FormControl(this.selectedEntity.isRequired || true),
      isUnique: new FormControl(this.selectedEntity.isUnique || true),
      note: new FormControl(this.selectedEntity.note || null),
    });
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