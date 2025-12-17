import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { ManufacturerInListDto, ManufacturersService } from '@proxy/catalog/manufacturers';
import { ProductCategoriesService, ProductCategoryInListDto } from '@proxy/catalog/product-categories';
import { ProductDto } from '@proxy/catalog/products';
import { ProductsService } from '@proxy/catalog/products';
import { productTypeOptions } from '@proxy/meta-king/products';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { UtilityService } from 'src/app/shared/services/utility.service';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  btnDisabled = false;
  public form: FormGroup;
  public thumbnailImage;

  productCategories: any[] = [];
  productCategoriesAll: ProductCategoryInListDto[] = [];
  parentCategories: any[] = [];
  childCategories: any[] = [];
  manufacturers: any[] = [];
  productTypes: any[] = [];
  selectedParentId: string | null = null;
  selectedEntity = {} as ProductDto;

  constructor(
    private productService: ProductsService,
    private productCategoryService: ProductCategoriesService,
    private manufacturerService: ManufacturersService,
    private fb: FormBuilder,
    private config: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private utilService: UtilityService,
    private notificationService: NotificationService,
    private cd: ChangeDetectorRef,
    private sanitizer: DomSanitizer
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
    this.loadProductTypes();
    this.initFormData();
  }

  generateSlug() {
    this.form.controls['slug'].setValue(this.utilService.MakeSeoTitle(this.form.get('name').value));
  }

  initFormData() {
    const productCategories = this.productCategoryService.getListAll();
    const manufacturers = this.manufacturerService.getListAll();

    this.toggleBlockUI(true);

    forkJoin({ productCategories, manufacturers })
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: (response) => {
        const productCategories = response.productCategories as ProductCategoryInListDto[];
        const manufacturers = response.manufacturers as ManufacturerInListDto[];

        this.productCategoriesAll = productCategories;

        this.parentCategories = productCategories
          .filter(x => x.parentId == null)
          .map(x => ({
            value: x.id,
            label: x.name,
          }));

        this.childCategories = [];

        this.manufacturers = manufacturers.map(x => ({
          value: x.id,
          label: x.name,
        }));

        if (this.utilService.isEmpty(this.config.data?.id)) {
          this.getNewSuggestionCode();
          this.toggleBlockUI(false);
        } else {
          this.loadFormDetails(this.config.data?.id);
        }
      },
      error: () => this.toggleBlockUI(false),
    });
  }

  onParentChanged(parentId: string) {
    this.selectedParentId = parentId;
    const children = this.productCategoriesAll.filter(x => (x as any).parentId === parentId);
    this.childCategories = children.map(x => ({ value: x.id, label: x.name }));
    if (this.childCategories.length === 0 && parentId) {
      const parent = this.productCategoriesAll.find(x => x.id === parentId);
      if (parent) {
        this.childCategories = [{ value: parent.id, label: parent.name }];
      }
    }
    this.form.patchValue({ categoryId: null });
  }

  getNewSuggestionCode() {
    this.productService
    .getSuggestNewCode()
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: (response: string) => {
        this.form.patchValue({
          code: response,
        });
      }
    });
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.productService
    .get(id)
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe({
      next: (response: ProductDto) => {
        this.selectedEntity = response;
        this.loadThumbnail(this.selectedEntity.thumbnailPicture);
        const cat = this.productCategoriesAll.find(x => x.id === this.selectedEntity.categoryId);
        if (cat) {
          const parentId = (cat as any).parentId as string | undefined;
          if (parentId) {
            this.selectedParentId = parentId;
            this.childCategories = this.productCategoriesAll
              .filter(x => (x as any).parentId === parentId)
              .map(x => ({ value: x.id, label: x.name }));
          } else {
            this.selectedParentId = null;
            this.childCategories = [];
          }
        }

        this.buildForm();
        this.form.patchValue({ parentCategoryId: this.selectedParentId || null, categoryId: this.selectedEntity.categoryId || null });
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
      this.productService
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
      this.productService
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

  loadProductTypes() {
    this.productTypes = productTypeOptions.map(x => ({
      label: x.key,
      value: x.value
    }));
  }

  private buildForm() {
    this.form = this.fb.group({
      name: new FormControl(
        this.selectedEntity.name || null,
        Validators.compose([Validators.required, Validators.maxLength(250)])
      ),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
      sku: new FormControl(this.selectedEntity.sku || null, Validators.required),
      manufacturerId: new FormControl(this.selectedEntity.manufacturerId || null,Validators.required),
      categoryId: new FormControl(this.selectedEntity.categoryId || null, Validators.required),
      parentCategoryId: new FormControl(this.selectedEntity['parentCategoryId'] || null),
      productType: new FormControl(this.selectedEntity.productType || null, Validators.required),
      sellPrice: new FormControl(this.selectedEntity.sellPrice || null, Validators.required),
      isVisibility: new FormControl(this.selectedEntity.isVisibility || true),
      isActive: new FormControl(this.selectedEntity.isActive || true),
      seoMetaDescription: new FormControl(this.selectedEntity.seoMetaDescription || null),
      description: new FormControl(this.selectedEntity.description || null),
      thumbnailPictureName: new FormControl(this.selectedEntity.thumbnailPicture || null),
      thumbnailPictureContent: new FormControl(null),
    });
  }

  loadThumbnail(fileName: string) {
    this.productService
      .getThumbnailImage(fileName)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: string) => {
          var fileExt = this.selectedEntity.thumbnailPicture?.split('.').pop();
          this.thumbnailImage = this.sanitizer.bypassSecurityTrustResourceUrl(
            `data:image/${fileExt};base64, ${response}`
          );
        },
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

  onFileChanged(event) {
    const reader = new FileReader();
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.form.patchValue({
          thumbnailPictureName: file.name,
          thumbnailPictureContent: reader.result,
        });
        this.cd.markForCheck();
      };
    }
  }
}
