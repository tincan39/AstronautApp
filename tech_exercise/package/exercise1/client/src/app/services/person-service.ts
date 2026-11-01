import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable, of } from 'rxjs';

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

interface GetPersonResponse {
  person: Person;
}

@Injectable({ providedIn: 'root' })
export class PersonService {
  private http = inject(HttpClient);

  getPeople(): Observable<Person[]> {
    return this.http.get<GetPeopleResponse>('api/Person').pipe(map(res => res.people));
  }

  getPerson(name: string): Observable<Person> {
    return this.http
      .get<GetPersonResponse>(`api/Person/${encodeURIComponent(name)}`)
      .pipe(map(res => res.person));
  }

  createPerson(name: string): Observable<Person> {
    return this.http
      .post<GetPersonResponse>('api/Person', { name })
      .pipe(map(res => res.person));
  }

  updatePerson(name: string | null, newName: string): Observable<void> {
    if(name == null)
      return of();

    return this.http.put<void>(`api/Person/${encodeURIComponent(name)}`, { name: newName });
  }
}
