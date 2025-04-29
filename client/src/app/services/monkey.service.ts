import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Monkey } from "../models/Monkey";

@Injectable({
  providedIn: 'root'
})
export class MonkeyService {
  private apiUrl = "http://localhost:5242/monkeys"

  constructor(

    private http: HttpClient
  ) {

  }


  getAll(): Observable<Monkey[]> {
    return this.http.get<Monkey[]>(this.apiUrl)
  }
}

