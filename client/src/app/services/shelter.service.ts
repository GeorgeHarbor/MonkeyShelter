import { Injectable } from "@angular/core";
import { environment } from "../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Shelter } from "../models/Shelter";

@Injectable({
  providedIn: 'root',
})
export class ShelterService {
  private apiUrl = `${environment.apiUrl}/shelters`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<Shelter[]> {
    return this.http.get<Shelter[]>(this.apiUrl);
  }
}
