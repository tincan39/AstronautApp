import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, DestroyRef, effect, inject, OnInit, signal } from '@angular/core';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import {MatCardModule} from '@angular/material/card';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { Person, PersonService } from '../../services/person-service';
import { LoadingOverlay } from '../../shared/loading-overlay/loading-overlay';
import { NotFound } from '../../shared/not-found/not-found';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-person-search',
  imports: [CommonModule, MatFormFieldModule, MatInputModule, MatIconModule, MatCardModule, ReactiveFormsModule, LoadingOverlay, NotFound, RouterLink],
  templateUrl: './person-search.html',
  styleUrl: './person-search.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PersonSearch implements OnInit {
  private personService = inject(PersonService);
  private destroyRef = inject(DestroyRef);
  people = signal<Person[]>([]);
  loading = signal(true);

  searchField = new FormControl({value: '', disabled: true}, { nonNullable: true })

  constructor() {
    effect(() => {
      if (this.loading()) {
        this.searchField.disable();
      } else {
        this.searchField.enable();
      }
    });
  }

  searchValue = toSignal(
    this.searchField.valueChanges.pipe(debounceTime(300), distinctUntilChanged()),
    { initialValue: this.searchField.value }
  );

  peopleToShow = computed(() =>
    this.people().filter(x => x.name.
    toLowerCase().startsWith(this.searchValue().toLowerCase())))

  ngOnInit(): void {
    this.personService.getPeople()
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(people => this.people.set(people));
  }

}
