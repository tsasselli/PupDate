import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { Injectable } from "@angular/core";
import { User } from "../_models/user";
import { Resolve, Router, ActivatedRoute, ActivatedRouteSnapshot } from "@angular/router";
import { UserService } from "../_services/user.service";
import { AlertifyService } from "../_services/alertify.service";

@Injectable()
export class ListsResolver implements Resolve<User[]> {

    pageNumber: number = 1;
    pageSize: number = 5;
    likesParameter = 'Likers';

    constructor(private userService: UserService,
        private router: Router, private alertify: AlertifyService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likesParameter).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving likes data from the db');
                this.router.navigate(['/home']);
                return of(null);
            })
        )
    }

}