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

  //Dropdown
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
    private notificationSerivce: NotificationService,
    private cd: ChangeDetectorRef,
    private sanitizer: DomSanitizer
  ) {}

  validationMessages = {
  code: [
    { type: 'required', message: 'Bạn Phải Nhập Mã Sản Phẩm' },
  ],
  name: [
    { type: 'required', message: 'Bạn Phải Nhập Tên Sản Phẩm' },
    { type: 'maxlength', message: 'Bạn Không Được Nhập Quá 255 Kí Tự' },
  ],
  slug: [
    { type: 'required', message: 'Bạn Phải Nhập Slug Sản Phẩm' },
  ],
  sku: [
    { type: 'required', message: 'Bạn Phải Nhập Mã SKU Sản Phẩm' },
  ],
  manufacturerId: [
    { type: 'required', message: 'Bạn Phải Chọn Nhà Sản Xuất' },
  ],
  categoryId: [
    { type: 'required', message: 'Bạn Phải Chọn Danh Mục Sản Phẩm' },
  ],
  productType: [
    { type: 'required', message: 'Bạn Phải Chọn Loại Sản Phẩm' },
  ],
  sortOrder: [
    { type: 'required', message: 'Bạn Phải Nhập Thứ Tự Sản Phẩm' },
  ],
  sellPrice: [
    { type: 'required', message: 'Bạn Phải Nhập Giá Bán' },
  ],
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
    this.loadProductTypes();
    this.initFormData();
  }

  generateSlug() {
    this.form.controls['slug'].setValue(this.utilService.MakeSeoTitle(this.form.get('name').value));
  }

  initFormData() {
    var productCategories = this.productCategoryService.getListAll();
    var manufacturers = this.manufacturerService.getListAll();
    this.toggleBlockUI(true);
    forkJoin({
      productCategories,
      manufacturers,
    })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: any) => {
          //Push data to dropdown
          var productCategories = response.productCategories as ProductCategoryInListDto[];
          var manufacturers = response.manufacturers as ManufacturerInListDto[];
          // Keep raw list for building parent/child relationships
          this.productCategoriesAll = productCategories;

          // Build parent categories (ParentId == null)
          this.parentCategories = productCategories
            .filter(x => (x as any).parentId == null || (x as any).parentId === undefined)
            .map(x => ({ value: x.id, label: x.name }));

          // Default childCategories empty; will populate when parent selected
          this.childCategories = [];

          manufacturers.forEach(element => {
            this.manufacturers.push({
              value: element.id,
              label: element.name,
            });
          });

          if (this.utilService.isEmpty(this.config.data?.id) == true) {
            this.getNewSuggestionCode();
            this.toggleBlockUI(false);
          } else {
            this.loadFormDetails(this.config.data?.id);
          }
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  
  onParentChanged(parentId: string) {
    this.selectedParentId = parentId;
    // find children whose parentId == parentId
    const children = this.productCategoriesAll.filter(x => (x as any).parentId === parentId);
    this.childCategories = children.map(x => ({ value: x.id, label: x.name }));
    // If no children, allow selecting the parent itself
    if (this.childCategories.length === 0 && parentId) {
      const parent = this.productCategoriesAll.find(x => x.id === parentId);
      if (parent) {
        this.childCategories = [{ value: parent.id, label: parent.name }];
      }
    }
    // reset categoryId when parent changes
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
            this.notificationSerivce.showError(err.error.error.message);
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
            this.notificationSerivce.showError(err.error.error.message);
            this.toggleBlockUI(false);
          },
        });
    }
  }

  loadProductTypes() {
    productTypeOptions.forEach(element => {
      this.productTypes.push({
        value: element.value,
        label: element.key,
      });
    });
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
      manufacturerId: new FormControl(
        this.selectedEntity.manufacturerId || null,
        Validators.required
      ),
      categoryId: new FormControl(this.selectedEntity.categoryId || null, Validators.required),
      parentCategoryId: new FormControl(this.selectedEntity['parentCategoryId'] || null),
      productType: new FormControl(this.selectedEntity.productType || null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || null, Validators.required),
      sellPrice: new FormControl(this.selectedEntity.sellPrice || null, Validators.required),
      visibility: new FormControl(this.selectedEntity.visibility || true),
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
