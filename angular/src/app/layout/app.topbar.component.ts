import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { LayoutService } from "./service/app.layout.service";
import { AuthService } from '../shared/services/auth.service';
import { Router } from '@angular/router';
import { LOGIN_URL } from '../shared/constants/urls.const';
import { NotificationService } from '../shared/services/notification.service';

@Component({
    selector: 'app-topbar',
    templateUrl: './app.topbar.component.html'
})
export class AppTopBarComponent implements OnInit {

    items!: MenuItem[];
    userMenuItems: MenuItem[];

    @ViewChild('menubutton') menuButton!: ElementRef;

    @ViewChild('topbarmenubutton') topbarMenuButton!: ElementRef;

    @ViewChild('topbarmenu') menu!: ElementRef;

    constructor(public layoutService: LayoutService, private authService: AuthService, private router: Router, private notificationService: NotificationService) { }
    ngOnInit(): void {
        this.userMenuItems = [
            {
                label: 'Thông Tin Cá Nhân',
                icon: 'pi pi-id-card',
                routerLink: ['/profile']
            },
            {
                label: 'Đổi Mật Khẩu',
                icon: 'pi pi-key',
                routerLink: ['/change-password']
            },
            {
                label: 'Đăng Xuất',
                icon: 'pi pi-sign-out',
                command: event => {
                    this.authService.logout();
                    this.router.navigate([LOGIN_URL]);
                    this.notificationService.showSuccess("Đăng Xuất Thành Công.")
                }
            }
        ]
    }
}
