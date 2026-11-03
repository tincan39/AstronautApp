import { ChangeDetectionStrategy, Component, computed, DestroyRef, inject, signal, viewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { filter, first, forkJoin, map, switchMap } from 'rxjs';
import { PersonService, Person } from '../../services/person-service';
import { AstronautDuty, AstronautDutyService } from '../../services/astronaut-duty-service';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import { FormControl, FormGroupDirective, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe, formatDate } from '@angular/common';
import { LoadingOverlay } from '../../shared/loading-overlay/loading-overlay';
import { StatusMessage, StatusType } from '../../shared/status-message/status-message';
import {MatButtonModule} from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDividerModule } from '@angular/material/divider';
import { NotFound } from '../../shared/not-found/not-found';
import { LoadingService } from '../../services/loading-service';
import { trackLoading } from '../../services/track-loading';

@Component({
  selector: 'app-person',
  imports: [MatFormFieldModule, MatInputModule, ReactiveFormsModule, DatePipe, LoadingOverlay, MatButtonModule, MatIconModule, RouterLink, StatusMessage, MatDatepickerModule, MatDividerModule, NotFound],
  providers: [provideNativeDateAdapter(), LoadingService],
  templateUrl: './person-page.html',
  styleUrl: './person-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PersonPage{
  private personService = inject(PersonService);
  private dutiesService = inject(AstronautDutyService);
  private loadingService = inject(LoadingService);

  private destroyRef = inject(DestroyRef);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(NonNullableFormBuilder);

  private dutyFormDir = viewChild(FormGroupDirective);
  dutyForm = this.fb.group({
    rank: ['', Validators.required],
    dutyTitle: ['', Validators.required],
    dutyStartDate: this.fb.control<Date | null>(null, Validators.required),
  });
  
  name =  new FormControl('', { nonNullable: true });

  currPerson = signal<Person | null>(null);
  astDuties = signal<AstronautDuty[]>([]);
  
  pastDuties = computed(() =>
    this.astDuties()
      .filter(d => d.dutyEndDate !== null)
      .sort((a, b) => b.dutyStartDate.localeCompare(a.dutyStartDate))
  );

  isLoading = this.loadingService.isLoading;
  statusMessage = signal<{ message: string; status: StatusType } | null>(null);
  dutyStatusMessage = signal<{ message: string; status: StatusType } | null>(null);

  constructor(){
    this.route.paramMap.pipe(
      first(),
      map(params => params.get("name")),
      filter((name): name is string => name !== null),
      switchMap(name => this.loadPersonAndDuties(name)),
      takeUntilDestroyed()
    )
    .subscribe({
      next: ([person, duties]) => this.setPersonAndDuties(person, duties),
      error: (err: HttpErrorResponse) =>
        this.statusMessage.set({ message: this.errorMessage(err, 'Could not load this person'), status: 'error' })
    })
  }


  loadPersonAndDuties = (name : string) =>{
    return forkJoin([this.personService.getPerson(name), this.dutiesService.getDuties(name)]).pipe(
      trackLoading(this.loadingService)
    );
  }

  addPerson = () =>{
    this.statusMessage.set(null);
    this.personService.createPerson(this.name.value.trim())
      .pipe(trackLoading(this.loadingService), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next:(res:Person) =>{
          this.setPersonAndDuties(res, []);
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
    this.personService.updatePerson(currName, newName)
      .pipe(trackLoading(this.loadingService), takeUntilDestroyed(this.destroyRef))
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

  addDuty = () =>{
    const person = this.currPerson();
    const { rank, dutyTitle, dutyStartDate } = this.dutyForm.getRawValue();

    if(person == null || this.dutyForm.invalid || dutyStartDate == null)
      return;

    this.dutyStatusMessage.set(null);
    this.dutiesService.createDuty({
        name: person.name,
        rank: rank.trim(),
        dutyTitle: dutyTitle.trim(),
        dutyStartDate: formatDate(dutyStartDate, 'yyyy-MM-dd', 'en-US'),
      })
      .pipe(
        trackLoading(this.loadingService),
        switchMap(() => this.loadPersonAndDuties(person.name)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: ([person, duties]) =>{
          this.setPersonAndDuties(person, duties);
          this.dutyFormDir()?.resetForm();
          this.dutyStatusMessage.set({ message: `Added duty ${dutyTitle.trim()}`, status: 'success' });
        },
        error: (err: HttpErrorResponse) =>
          this.dutyStatusMessage.set({ message: this.errorMessage(err, 'Could not add this duty'), status: 'error' })
      });
  }

  private setPersonAndDuties(person: Person, duties: AstronautDuty[]): void {
    this.currPerson.set(person);
    this.name.setValue(person.name);
    this.astDuties.set(duties);
  }

  private errorMessage(err: HttpErrorResponse, fallback: string): string {
    return err.error?.detail ?? fallback;
  }
}
