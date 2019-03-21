import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { FlexLayoutModule } from '@angular/flex-layout';
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { APP_ROUTING } from "../_helpers/constants/app.routing";
import { MyMaterialModule } from "../modules/material/my-material.module";
import {
  NotificationService,
  StorageService,
  AuthenticationService
} from "../services";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { HomeComponent } from "./home/home.component";
import { LoginComponent } from "./login/login.component";
import { NavBarComponent } from "./_commons/nav-bar/nav-bar.component";
import {AuthGuard} from '../_guards';
import { UsersComponent } from './users/users.component';
import { MyTableComponent } from './my-table/my-table.component';
import { ParkingsComponent } from './parkings/parkings.component';
import { EventsComponent } from './events/events.component';
import { OrdersComponent } from './orders/orders.component';
import { UserDeleteConfirmDialogComponent } from './_shared/delete-confirm-dialog/user-delete-confirm-dialog.component';
// import { JwtInterceptor, ErrorInterceptor } from "../components/_helpers";

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    HomeComponent,
    LoginComponent,
    UsersComponent,
    MyTableComponent,
    ParkingsComponent,
    EventsComponent,
    OrdersComponent,
    UserDeleteConfirmDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    MyMaterialModule,
    HttpClientModule,
    APP_ROUTING,
    FlexLayoutModule
  ],
  providers: [
    NotificationService,
    StorageService,
    AuthenticationService,
    AuthGuard
  ],
  bootstrap: [AppComponent],
  entryComponents: [UserDeleteConfirmDialogComponent],
  exports: []
})
export class AppModule {}
