import { EventsComponent } from './../../components/events/events.component';
import { ParkingsComponent } from './../../components/parkings/parkings.component';
import { MyTableComponent } from './../../components/my-table/my-table.component';
import {Route, RouterModule} from '@angular/router';
import {HomeComponent} from '../../components/home/home.component';
import {LoginComponent} from '../../components/login/login.component';
import {AuthGuard} from '../../_guards';
import { UsersComponent } from 'src/app/components/users/users.component';
import { OrdersComponent } from 'src/app/components/orders/orders.component';

const APP_ROUTES: Route[] = [
  { path: '', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'myTable', component: MyTableComponent },
  { path: 'login', component: LoginComponent },
  { path: 'users', component: UsersComponent },
  { path: 'parkings', component: ParkingsComponent },
  { path: 'orders', component: OrdersComponent },
  { path: 'events', component: EventsComponent },
  { path: '**', redirectTo: '' }
];

export const APP_ROUTING = RouterModule.forRoot(APP_ROUTES);
