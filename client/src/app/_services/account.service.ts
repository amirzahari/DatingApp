import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})

export class AccountService {
  
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http:HttpClient) {
    //console.log("+++ [START] AccountService : constructor()");
    //console.log("this.currentUserSource");
    //console.log(this.currentUserSource);
    //console.log("this.currentUser$");
    //console.log(this.currentUser$);
    //console.log("+++ [END] AccountService : constructor()");
  }

  login(model:any){
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response:User) => {
        const user = response;
        if(user){
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  register(model:any){
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map(
        (user:User) => {
          if(user){
            this.setCurrentUser(user);
          }
        }
      )
    );
  }

  registerReturnUser(model:any){
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map(
        (user:User) => {
          if(user){
            this.setCurrentUser(user);
            //this.currentUserSource.next(user);
          }
          return user;
        }
      )
    );
  }

  setCurrentUser(user: User){
    //console.log("+++ [START] AccountService : setCurrentUser()");
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
    // console.log("Set user to currentUserSource.next(user)");
    // console.log("+++ [END] AccountService : setCurrentUser()");
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
