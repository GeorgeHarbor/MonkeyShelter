// reports.service.ts
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Report } from "../models/Report";

@Injectable({ providedIn: 'root' })
export class ReportsService {
  private apiUrl = "http://localhost:5109";

  constructor(private http: HttpClient) { }

  getAll(): Observable<Report[]> {
    // fetch as a dictionary
    return this.http
      .get<Record<string, number>>(`${this.apiUrl}/reports/count-per-species`)
      .pipe(
        map(dict =>
          Object.entries(dict).map(
            ([species, count]) => ({ species, count } as Report)
          )
        )
      );
  }
  getInRange(from: string, to: string): Observable<Report[]> {
    return this.http
      .get<Record<string, number>>(
        `${this.apiUrl}/reports/arrivals?from=${from}&to=${to}`
      )
      .pipe(
        map(dict => Object.entries(dict).map(([species, count]) => ({ species, count })))
      );
  }
}
