import { HttpClient, HttpHeaders, HttpParams, JsonpClientBackend } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
// import { userInfo } from 'os';
import { environment } from 'src/environments/environment';
import { PhotoEditorComponent } from '../members/photo-editor/photo-editor.component';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

// *** [aznote] we use jwt.interceptor for every request to set the header token.

@Injectable({
  providedIn: 'root'
})

export class MembersService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User;
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })    
  }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params: UserParams){
    this.userParams = params;
  }

  resetUserParams(){
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: UserParams) {

    var cacheKey = Object.values(userParams).join('-');
    var cacheValue = this.memberCache.get(cacheKey);
    if (cacheValue) {
      return of(cacheValue);
    }

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
      .pipe(map( response => {
        this.memberCache.set(cacheKey, response);
        return response;
    }));
  }

  getMember(username: string) {
    
    // *** Get the cache data of memberlist cache.
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.username === username);
    if(member){
      return of(member);
    }

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

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId, {});
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLike(predicate: string, pageNumber, pageSize) {

    //let params = this.getPaginationHeaders(pageNumber,pageSize);
    let params = getPaginationHeaders(pageNumber,pageSize);
    params = params.append('predicate', predicate);

    //return this.getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params);
    return getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params, this.http);

    //return this.http.get<Partial<Member[]>>(this.baseUrl + 'likes?predicate=' + predicate)
  }

  // private getPaginatedResult<T>(url, params) {
  //   const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
  //   return this.http.get<T>(url, { observe: 'response', params }).pipe(
  //     map(response => {
  //       paginatedResult.result = response.body;
  //       if (response.headers.get('Pagination') !== null) {
  //         paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
  //       }
  //       return paginatedResult;
  //     })
  //   );
  // }

  // private getPaginationHeaders(pageNumber: number, pageSize: number) {
  //   let params = new HttpParams();

  //   params = params.append('pageNumber', pageNumber.toString());
  //   params = params.append('pageSize', pageSize.toString());

  //   return params;
  // }

}
