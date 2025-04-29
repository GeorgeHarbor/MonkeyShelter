import { HttpClient, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { AuthResponse, UserLoginDto, UserRegisterDto } from "../models/dtos";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'http://localhost:5270/auth';

  constructor(private http: HttpClient) { }

  login(credentials: UserLoginDto): Observable<any> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials);
  }


  register(user: UserRegisterDto): Observable<HttpResponse<any>> {
    return this.http.post<any>(
      `${this.apiUrl}/register`,
      user,
      { observe: 'response' }
    );
  }

  isLoggedIn() {
    return !!localStorage.getItem("auth_token");
  }
}
