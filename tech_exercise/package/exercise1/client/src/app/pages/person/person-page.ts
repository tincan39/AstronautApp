import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { filter, finalize, first, forkJoin, map, switchMap } from 'rxjs';
import { PersonService, Person } from '../../services/person-service';
import { AstronautDuty, AstronautDutyService } from '../../services/astronaut-duty-service';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { LoadingOverlay } from '../../shared/loading-overlay/loading-overlay';
import { StatusMessage, StatusType } from '../../shared/status-message/status-message';
import {MatButtonModule} from '@angular/material/button';

@Component({
  selector: 'app-person',
  imports: [MatFormFieldModule, MatInputModule, ReactiveFormsModule, DatePipe, LoadingOverlay, MatButtonModule, MatIconModule, RouterLink, StatusMessage],
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
  statusMessage = signal<{ message: string; status: StatusType } | null>(null);

  constructor(){
    // Read the route once; after that, add/update keep the page state in sync themselves.
    this.route.paramMap.pipe(
      first(),
      map(params => params.get("name")),
      filter((name): name is string => name !== null),
      switchMap(name => this.loadPersonAndDuties(name)),
      takeUntilDestroyed()
    )
    .subscribe({
      next: ([person, duties]) =>{
        this.currPerson.set(person);
        this.name.setValue(person.name);
        this.astDuties.set(duties);
      },
      error: (err: HttpErrorResponse) =>
        this.statusMessage.set({ message: this.errorMessage(err, 'Could not load this person'), status: 'error' })
    })
  }


  loadPersonAndDuties = (name : string) =>{
    this.isLoading.set(true);
    return forkJoin([this.personService.getPerson(name), this.dutiesService.getDuties(name)]).pipe(
      finalize(() => this.isLoading.set(false))
    );
  }

  addPerson = () =>{
    this.statusMessage.set(null);
    this.isLoading.set(true);
    this.personService.createPerson(this.name.value.trim())
      .pipe(finalize(() => this.isLoading.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next:(res:Person) =>{
          this.currPerson.set(res);
          this.astDuties.set([]);
          this.name.setValue(res.name);
          this.statusMessage.set({ message: `Created ${res.name}`, status: 'success' });
        },
        error: (err: HttpErrorResponse) =>
          this.statusMessage.set({ message: this.errorMessage(err, 'Could not create this person'), status: 'error' })
      });
  }

  updatePerson = () =>{
    const currName = this.currPerson()?.name;
    
    if(currName == null)
      return;
    
    const newName = this.name.value.trim();
    this.statusMessage.set(null);
    this.isLoading.set(true);
    this.personService.updatePerson(currName, newName)
      .pipe(finalize(() => this.isLoading.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next:() =>{
          this.currPerson.update(p => p && { ...p, name: newName });
          this.statusMessage.set({ message: `Updated name to ${newName}`, status: 'success' });
          this.router.navigate(['/person', newName], { replaceUrl: true });
        },
        error: (err: HttpErrorResponse) =>
          this.statusMessage.set({ message: this.errorMessage(err, 'Could not update this person'), status: 'error' })
      });
  }

  private errorMessage(err: HttpErrorResponse, fallback: string): string {
    return err.error?.detail ?? fallback;
  }
}
