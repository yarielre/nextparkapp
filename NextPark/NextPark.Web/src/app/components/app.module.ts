import {HttpClientModule} from '@angular/common/http';
import {FlexLayoutModule} from '@angular/flex-layout';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {BrowserModule} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {APP_ROUTING} from '../_helpers/constants/app.routing';
import {MyMaterialModule} from '../modules/material/my-material.module';
import {NotificationService} from '../services/notification.service';
import {StorageService} from '../services/storage.service';
import {AuthenticationService} from '../services/authentication.service';
import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HomeComponent} from './home/home.component';
import {LoginComponent} from './login/login.component';
import {NavBarComponent} from './_commons/nav-bar/nav-bar.component';
import {AuthGuard} from '../_guards';
import {UsersComponent} from './users/users.component';
import {ParkingsComponent} from './parkings/parkings.component';
import {EventsComponent} from './events/events.component';
import {OrdersComponent} from './orders/orders.component';
import {UserDeleteConfirmDialogComponent} from './_shared/delete-confirm-dialog/user-delete-confirm-dialog.component';
import {ParkingDeleteConfirmDialogComponent} from './_shared/delete-confirm-dialog/parking-delete-confirm-dialog.component.1';
import {UserFormComponent} from './_forms/user-form/user-form.component';
import {EmailSenderComponent} from './_forms/email-sender/email-sender.component';
import {ParkingFormComponent} from './_forms/parking-form/parking-form.component';
import {TransactionsComponent} from './transactions/transactions.component';
import { TransactionFormComponent } from './_forms/transaction-form/transaction-form.component';
import { TransactionDeleteConfirmDialogComponent } from './_shared/delete-confirm-dialog/transaction-delete-confirm-dialog.component';

// import { JwtInterceptor, ErrorInterceptor } from "../components/_helpers";

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    HomeComponent,
    LoginComponent,
    UsersComponent,
    ParkingsComponent,
    EventsComponent,
    OrdersComponent,
    UserDeleteConfirmDialogComponent,
    ParkingDeleteConfirmDialogComponent,
    UserFormComponent,
    EmailSenderComponent,
    ParkingFormComponent,
    TransactionsComponent,
    TransactionFormComponent,
    TransactionDeleteConfirmDialogComponent
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
  entryComponents: [
    UserDeleteConfirmDialogComponent,
    ParkingDeleteConfirmDialogComponent,
    UserFormComponent,
    ParkingFormComponent,
    EmailSenderComponent,
    TransactionFormComponent,
    TransactionDeleteConfirmDialogComponent
  ],
  exports: []
})
export class AppModule {
}
