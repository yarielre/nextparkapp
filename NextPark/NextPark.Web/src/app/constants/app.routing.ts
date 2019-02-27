import {Route, RouterModule} from '@angular/router';
import {HomeComponent} from '../components/home/home.component';
import {LoginComponent} from '../components/login/login.component';
import {AuthGuard} from '../components/_guards';

const APP_ROUTES: Route[] = [
  { path: '', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: '**', redirectTo: '' }
];

export const APP_ROUTING = RouterModule.forRoot(APP_ROUTES);
