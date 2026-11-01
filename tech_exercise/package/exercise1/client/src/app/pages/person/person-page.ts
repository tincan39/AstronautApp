import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { filter, forkJoin, map,  switchMap } from 'rxjs';
import { PersonService, Person } from '../../services/person-service';
import { AstronautDuty, AstronautDutyService } from '../../services/astronaut-duty-service';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import { ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { LoadingOverlay } from '../../shared/loading-overlay/loading-overlay';

@Component({
  selector: 'app-person',
  imports: [MatFormFieldModule, MatInputModule, ReactiveFormsModule, DatePipe, LoadingOverlay],
  templateUrl: './person-page.html',
  styleUrl: './person-page.css',
})
export class PersonPage{
  private personService = inject(PersonService);
  private dutiesService = inject(AstronautDutyService);
  private destroyRef = inject(DestroyRef);

  private route = inject(ActivatedRoute);
  name = toSignal(this.route.paramMap.pipe(map(params => params.get('name'))), { initialValue: null });

  currPerson = signal<Person | null>(null);
  astDuties = signal<AstronautDuty[]>([]);

  constructor(){
    this.route.paramMap.pipe(
      map(params => params.get("name")),
      filter((name): name is string => name !== null),
      switchMap(name => forkJoin([this.personService.getPerson(name), this.dutiesService.getDuties(name)])),
      takeUntilDestroyed()
    )
    .subscribe({
      next: ([person, duties] : [Person, AstronautDuty[]]) =>{
        this.currPerson.set(person);
        this.astDuties.set(duties);
      }
    })
  }

}
