import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Species } from "../models/Species";

@Injectable({
  providedIn: 'root'
})
export class SpeciesService {
  private apiUrl = "http://localhost:5242/species"

  constructor(

    private http: HttpClient
  ) {

  }

  getAll(): Observable<Species[]> {
    return this.http.get<Species[]>(this.apiUrl)
  }
}

