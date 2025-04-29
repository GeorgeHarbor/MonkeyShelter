import { Injectable } from "@angular/core";
import { environment } from "../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class ShelterService {
  private apiUrl = `${environment.apiUrl}/shelters`;

  constructor(private http: HttpClient) { }

  getShelters(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
}
