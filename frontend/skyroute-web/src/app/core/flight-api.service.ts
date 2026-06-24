import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Airport, BookingRequest, BookingResponse, SearchCriteria, SearchResponse } from './models';

/** Thin wrapper over the backend HTTP API. Requests use the relative `/api` path; the dev server
 *  proxies it to the .NET backend (see proxy.conf.json), so the browser stays same-origin. */
@Injectable({ providedIn: 'root' })
export class FlightApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api';

  airports(): Observable<Airport[]> {
    return this.http.get<Airport[]>(`${this.baseUrl}/airports`);
  }

  search(criteria: SearchCriteria): Observable<SearchResponse> {
    return this.http.post<SearchResponse>(`${this.baseUrl}/flights/search`, criteria);
  }

  book(request: BookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(`${this.baseUrl}/bookings`, request);
  }
}
