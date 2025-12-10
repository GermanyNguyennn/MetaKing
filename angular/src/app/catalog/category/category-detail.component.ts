import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { ProductCategoryDto, ProductCategoriesService } from '@proxy/catalog/product-categories';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { UtilityService } from 'src/app/shared/services/utility.service'; 

@Component({
  selector: 'app-category-detail',
  templateUrl: './category-detail.component.html',
})
export class CategoryDetailComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  btnDisabled = false;
  public form: FormGroup;
  public coverPicture;

  dataTypes: any[] = [];
  selectedEntity = {} as ProductCategoryDto;
  categoryOptions: any[] = [];

  constructor(
    private categoryService: ProductCategoriesService,
    private fb: FormBuilder,
    private config: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private utilService: UtilityService,
    private notificationService: NotificationService,
    private cd: ChangeDetectorRef,
    private sanitizer: DomSanitizer
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
    this.loadParentCategories();
    this.initFormData();
  }

  loadParentCategories() {
  this.categoryService
    .getList({
      maxResultCount: 1000,
      skipCount: 0,
    })
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: (response) => {
        const currentId = this.config.data?.id;

        this.categoryOptions = response.items.filter(x => x.id !== currentId);
      }
    });
  }

  generateSlug() {
    this.form.controls['slug'].setValue(this.utilService.MakeSeoTitle(this.form.get('name').value));
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
    this.categoryService
    .get(id)
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: (response: ProductCategoryDto) => {
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
      this.categoryService
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
      this.categoryService
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

  private buildForm() {
    this.form = this.fb.group({
      name: new FormControl(
        this.selectedEntity.name || null,
        Validators.compose([Validators.required, Validators.maxLength(250)])
      ),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
      isVisibility: new FormControl(this.selectedEntity.isVisibility || true),
      isActive: new FormControl(this.selectedEntity.isActive || true),
      parentId: new FormControl(this.selectedEntity.parentId || null),
      coverPictureName: new FormControl(this.selectedEntity.coverPicture || null),
      coverPictureContent: new FormControl(null),
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

  loadThumbnail(fileName: string) {
    this.categoryService
    .getThumbnailImage(fileName)
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: (response: string) => {
        var fileExt = this.selectedEntity.coverPicture?.split('.').pop();
        this.coverPicture = this.sanitizer.bypassSecurityTrustResourceUrl(
          `data:image/${fileExt};base64, ${response}`
        );
      },
    });
  }

  onFileChanged(event) {
    const reader = new FileReader();
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.form.patchValue({
          coverPictureName: file.name,
          coverPictureContent: reader.result,
        });
        this.cd.markForCheck();
      };
    }
  }
}