import { ActivatedRoute } from '@angular/router';
import { UserService } from './../_services/user.service';
import { Pagination, PaginationResult } from './../_models/pagination';
import { User } from './../_models/user';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  
  users: User[];
  pagination: Pagination;
  likesParameter: string;

  constructor(private userService: UserService, private authService: AuthService,
      private actRoute: ActivatedRoute, private alertify: AlertifyService) { }

  ngOnInit() {
    this.actRoute.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, 
      this.pagination.itemsPerPage, null, this.likesParameter)
        .subscribe((result: PaginationResult<User[]>) => {
        this.users = result.result;
        this.pagination = result.pagination;
    }, error => {
      this.alertify.error(error);
    })
  }

}
