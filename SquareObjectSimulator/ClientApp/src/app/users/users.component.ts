import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html'
})
export class UsersComponent {
    public userlist: UserVO[];
    private baseUrl: string;
    addnewid: string;

    constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
        this.getList();
    }

    public getList() {
        
        this.userlist = null;
        this.http.get<UserVO[]>(this.baseUrl + 'api/userinfo').subscribe(result => {
            this.userlist = result;
        }, error => console.error(error));
    }

    public addnew() {

        this.http.get<UserVO>(this.baseUrl + 'api/userinfo/add/' + this.addnewid).subscribe(result => {
            this.userlist.push(result);
        }, error => console.error(error));
    }

    public delete(index: number) {
        this.http.get<UserVO>(this.baseUrl + 'api/userinfo/del/' + this.userlist[index].id).subscribe(result => {
            this.userlist.splice(index,1);
        }, error => console.error(error));

    }
}

interface UserVO {
  id: string;
  soCount: number;
}
