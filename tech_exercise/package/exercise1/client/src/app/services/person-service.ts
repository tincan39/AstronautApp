import { HttpClient, HttpHeaders } from '@angular/common/http';
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

  createPerson(name: string): Observable<void> {
    // The API binds the body as a raw JSON string ([FromBody] string name),
    // so it must be sent as JSON rather than HttpClient's default text/plain.
    return this.http.post<void>('api/Person', JSON.stringify(name), {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    });
  }

  updatePerson(name: string, newName: string): Observable<void> {
    return this.http.put<void>(`api/Person/${encodeURIComponent(name)}`, { name: newName });
  }
}
