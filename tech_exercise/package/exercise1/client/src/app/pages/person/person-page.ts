import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { filter, finalize, first, forkJoin, map, switchMap } from 'rxjs';
import { PersonService, Person } from '../../services/person-service';
import { AstronautDuty, AstronautDutyService } from '../../services/astronaut-duty-service';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { LoadingOverlay } from '../../shared/loading-overlay/loading-overlay';
import {MatButtonModule} from '@angular/material/button';

@Component({
  selector: 'app-person',
  imports: [MatFormFieldModule, MatInputModule, ReactiveFormsModule, DatePipe, LoadingOverlay, MatButtonModule, MatIconModule, RouterLink],
  templateUrl: './person-page.html',
  styleUrl: './person-page.css',
})
export class PersonPage{
  private personService = inject(PersonService);
  private dutiesService = inject(AstronautDutyService);
  
  private destroyRef = inject(DestroyRef);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  name =  new FormControl('', { nonNullable: true });

  currPerson = signal<Person | null>(null);
  astDuties = signal<AstronautDuty[]>([]);
  isLoading = signal(false);

  constructor(){
    this.route.paramMap.pipe(
      first(),
      map(params => params.get("name")),
      filter((name): name is string => name !== null),
      switchMap(name => this.loadPersonAndDuties(name)),
      takeUntilDestroyed()
    )
    .subscribe({
      next: ([person, duties] : [Person, AstronautDuty[]]) =>{
        this.currPerson.set(person);
        this.astDuties.set(duties);
      }
    })
  }


  loadPersonAndDuties = (name : string) =>{
    this.isLoading.set(true);
    return forkJoin([this.personService.getPerson(name), this.dutiesService.getDuties(name)]).pipe(
      finalize(() => this.isLoading.set(false))
    );
  }

  addPerson = () =>{
    this.isLoading.set(true);
    this.personService.createPerson(this.name.value)
      .pipe(finalize(() => this.isLoading.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next:(res:Person) =>{
          this.currPerson.set(res);
          this.name.setValue(res.name);
          this.astDuties.set([]);
          this.router.navigate(["/person", res.name]);
        }
      });
  }

  updatePerson = () =>{
    this.isLoading.set(true);
    const currName = this.currPerson()?.name;
    const newName = this.name.value;

    if(currName == null)
      return;

    this.personService.updatePerson(currName, newName)
      .pipe(finalize(() => this.isLoading.set(false)), takeUntilDestroyed(this.destroyRef))
     .subscribe({
        next:() =>{
          this.router.navigate(["/person", newName]);
        }
      });
  }

}
