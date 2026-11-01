import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

export interface Person {
  name: string;
  currentRank: string;
  careerStartDate: string | null;
  careerEndDate: string | null;
  currentDutyTitle: string;
}

interface GetPeopleResponse {
  people: Person[];
}

@Injectable({ providedIn: 'root' })
export class PersonService {
  private http = inject(HttpClient);

  getPeople(): Observable<Person[]> {
    return this.http.get<GetPeopleResponse>('api/Person').pipe(map(res => res.people));
  }
}
