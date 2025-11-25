import { OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { LayoutService } from './service/app.layout.service';

@Component({
    selector: 'app-menu',
    templateUrl: './app.menu.component.html'
})
export class AppMenuComponent implements OnInit {

    model: any[] = [];

    constructor(public layoutService: LayoutService) { }

    ngOnInit() {
        this.model = [
            {
                label: 'Trang Chủ',
                items: [
                    { label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/'] }
                ]
            },
            {
                label: 'Sản Phẩm',
                items: [
                    { label: 'Danh Sách Sản Phẩm', icon: 'pi pi-fw pi-id-card', routerLink: ['/catalog/product'] },
                    { label: 'Danh Sách Thuộc Tính', icon: 'pi pi-fw pi-id-card', routerLink: ['/catalog/attribute'] },
                    { label: 'Danh Sách Danh Mục', icon: 'pi pi-fw pi-id-card', routerLink: ['/catalog/category'] },
                ]
            },
            {
                label: 'Hệ Thống',
                items: [
                    { label: 'Danh Sách Quyền', icon: 'pi pi-fw pi-id-card', routerLink: ['/system/role'] },
                    { label: 'Danh Sách Người Dùng', icon: 'pi pi-fw pi-id-card', routerLink: ['/system/user'] },
                ]
            },           
        ];
    }
}
