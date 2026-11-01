import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import {MatCardModule} from '@angular/material/card';

interface Person {
  name:string;
  currentRank:string;
  careerStartDate:string | null;
  careerEndDate:string | null;
  currentDutyTitle: string
}

@Component({
  selector: 'app-person-search',
  imports: [CommonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatCardModule],
  templateUrl: './person-search.html',
  styleUrl: './person-search.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PersonSearch implements OnInit {

  http = inject(HttpClient);
  people = signal<Person[]>([]);

  ngOnInit(): void {
    this.http.get("api/Person")
      .subscribe((res:any) =>{
        const personList = res.people as Person[];
        this.people.set(personList);
        console.log(res);
      });
  }

}
