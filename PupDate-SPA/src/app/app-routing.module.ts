import { AuthGuard } from './_guard/auth.guard';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { HomeComponent } from './home/home.component';
import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';

const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  { 
    path: '', 
    runGuardsAndResolvers:'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members', component: MemberListComponent },
      { path: 'members/:id', component: MemberDetailComponent },
      { path: 'messages', component: MessagesComponent },
      { path: 'lists', component: ListsComponent },

    ]
  },
  { path: '**', redirectTo: '', pathMatch: 'full' },
]

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule ]
})
export class AppRoutingModule { }
 