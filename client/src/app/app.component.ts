import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'The Dating App';
  users: any;

  // constructor(private http: HttpClient, private accountService: AccountService){}
  constructor(private accountService: AccountService) { }

  ngOnInit() {
    // this.getUsers();
    this.setCurrentUser();
  }

  setCurrentUser() {
    //console.log("+++ [START] AppComponent : setCurrentUser()");
    //console.log("Get current user from local storage.")
    const user: User = JSON.parse(localStorage.getItem('user'));
    //console.log(user);
    //console.log("call setCurrentUser from accountService and set user")
    this.accountService.setCurrentUser(user);
    //console.log("+++ [END] AppComponent : setCurrentUser()");
  }

  // getUsers () {
  //   this.http.get("https://localhost:5001/api/users").subscribe ( 
  //     response => { 
  //       this.users = response 
  //     },
  //     error => {
  //       console.log(error)
  //     }
  //   )
  // }

  // getUsers() {
  //   console.log("Calling the list of users inside..")
  //   this.http.get('https://localhost:5001/api/users').subscribe({
  //     next: response => this.users = response,
  //     error: error => console.log(error)
  //   })
  // }

}
