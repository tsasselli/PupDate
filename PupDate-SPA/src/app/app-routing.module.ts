import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { AuthGuard } from './_guard/auth.guard';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { HomeComponent } from './home/home.component';
import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';

const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  { 
    path: '', 
    runGuardsAndResolvers:'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members', component: MemberListComponent, resolve: 
        { users: MemberListResolver } 
      },
      { path: 'members/:id', component: MemberDetailComponent, resolve: { user: MemberDetailResolver } },
      { path: 'member/edit', component: MemberEditComponent, resolve: { user: MemberEditResolver }},
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
 