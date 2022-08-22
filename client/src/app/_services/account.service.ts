import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})

export class AccountService {

  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presence: PresenceService) {
    //console.log("+++ [START] AccountService : constructor()");
    //console.log("this.currentUserSource");
    //console.log(this.currentUserSource);
    //console.log("this.currentUser$");
    //console.log(this.currentUser$);
    //console.log("+++ [END] AccountService : constructor()");
  }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
          // localStorage.setItem('user', JSON.stringify(user));
          // this.currentUserSource.next(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map(
        (user: User) => {
          if (user) {
            this.setCurrentUser(user);
            this.presence.createHubConnection(user);
          }
        }
      )
    );
  }

  // registerReturnUser(model: any) {
  //   return this.http.post(this.baseUrl + 'account/register', model).pipe(
  //     map(
  //       (user: User) => {
  //         if (user) {
  //           this.setCurrentUser(user);
  //           //this.currentUserSource.next(user);
  //         }
  //         return user;
  //       }
  //     )
  //   );
  // }

  setCurrentUser(user: User) {
    //console.log(" ### setCurrentUser : " , user);
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    //console.log(" ### user roles : " , roles);

    //if(roles != null)
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
    // console.log("Set user to currentUserSource.next(user)");
    // console.log("+++ [END] AccountService : setCurrentUser()");
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presence.stopHubConnection();
  }

  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
