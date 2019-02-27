import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { APP_ROUTING } from "../constants/app.routing";
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
import {AuthGuard} from './_guards';
// import { JwtInterceptor, ErrorInterceptor } from "../components/_helpers";

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    HomeComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    MyMaterialModule,
    HttpClientModule,
    APP_ROUTING
  ],
  providers: [
    NotificationService,
    StorageService,
    AuthenticationService,
    AuthGuard
    // { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    // { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent],
  entryComponents: [],
  exports: []
})
export class AppModule {}
