import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  constructor(private http: HttpClient) { }

  private url = 'https://localhost:7255/winelottery';

  postData(data: any): Observable<any> {
    return this.http.post(this.url, data);
  }

  getNumberByTicket(ticket: number): Observable<any> {
    return this.http.get<any>(`${this.url}/${ticket}`);
  }
}
