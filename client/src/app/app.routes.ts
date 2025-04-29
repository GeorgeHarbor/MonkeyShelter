import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AuthGuard } from './utils/add-guard';
import { MonkeyListComponent } from './components/dashboard/monkey-list/monkey-list.component';
import { ReportsComponent } from './components/dashboard/reports/reports.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: LoginComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    children: [
      { path: '', component: MonkeyListComponent },
      { path: 'reports', component: ReportsComponent }
    ],
    canActivate: [AuthGuard]
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];


