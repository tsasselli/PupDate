import { Pagination, PaginationResult } from './../../_models/pagination';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../_models/user';
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  userParameters: any = {};
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];
  pagination: Pagination;

  constructor(private userService: UserService, private alertify: AlertifyService, 
              private route: ActivatedRoute,) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.userParameters.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParameters.minAge = 18;
    this.userParameters.maxAge = 99;
    this.userParameters.orderBy = 'lastActive';
  }

  resetFilters() {
    this.userParameters.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParameters.minAge = 18;
    this.userParameters.maxAge = 99;
    this.loadUsers();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParameters).subscribe((result: PaginationResult<User[]>) => {
      this.users = result.result;
      this.pagination = result.pagination;
    }, error => {
      this.alertify.error(error);
    })
  }

}
