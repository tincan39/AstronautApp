import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

export interface AstronautDuty {
  rank: string;
  dutyTitle: string;
  dutyStartDate: string;
  dutyEndDate: string | null;
}

export interface CreateAstronautDutyRequest {
  name: string;
  rank: string;
  dutyTitle: string;
  /** yyyy-MM-dd */
  dutyStartDate: string;
}

interface GetAstronautDutiesResponse {
  astronautName: string;
  astronautDuties: AstronautDuty[];
}

@Injectable({ providedIn: 'root' })
export class AstronautDutyService {
  private http = inject(HttpClient);

  getDuties(name: string): Observable<AstronautDuty[]> {
    return this.http
      .get<GetAstronautDutiesResponse>(`api/AstronautDuty/${encodeURIComponent(name)}`)
      .pipe(map(res => res.astronautDuties));
  }

  createDuty(request: CreateAstronautDutyRequest): Observable<void> {
    return this.http.post<void>('api/AstronautDuty', request);
  }
}
