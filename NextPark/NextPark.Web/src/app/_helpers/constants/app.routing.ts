import { EventsComponent } from './../../components/events/events.component';
import { ParkingsComponent } from './../../components/parkings/parkings.component';
import {Route, RouterModule} from '@angular/router';
import {HomeComponent} from '../../components/home/home.component';
import {LoginComponent} from '../../components/login/login.component';
import {AuthGuard} from '../../_guards';
import { UsersComponent } from 'src/app/components/users/users.component';
import { OrdersComponent } from 'src/app/components/orders/orders.component';
import {TransactionsComponent} from '../../components/transactions/transactions.component';

const APP_ROUTES: Route[] = [
  { path: '', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'users', component: UsersComponent },
  { path: 'parkings', component: ParkingsComponent },
  { path: 'orders', component: OrdersComponent },
  { path: 'events', component: EventsComponent },
  { path: 'transactions', component: TransactionsComponent },
  { path: '**', redirectTo: '' }
];

export const APP_ROUTING = RouterModule.forRoot(APP_ROUTES);
