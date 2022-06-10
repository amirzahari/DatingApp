import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
// import { userInfo } from 'os';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

// *** [aznote] we use jwt.interceptor for every request to set the header token.
// const httpOptions = {
//   headers: new HttpHeaders({
//     Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token
//   })
// }

@Injectable({
  providedIn: 'root'
})

export class MembersService {

  baseUrl = environment.apiUrl;
  members:Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    if(this.members.length > 0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;
      })
    );
  }

  getMember(username: string) {
    const member = this.members.find(x => x.username === username)
    if(member !== undefined) return of (member)
    return this.http.get<Member>(this.baseUrl + 'users/' + username)
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + "users", member).pipe(
      map(() => {
        console.log(member);
        const index = this.members.indexOf(member);
        console.log(index);
        this.members[index] = member;
        console.log(this.members[index]);
      })
    );
  }
  
}
