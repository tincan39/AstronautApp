import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { PersonSearch } from './person-search';

describe('PersonSearch', () => {
  let component: PersonSearch;
  let fixture: ComponentFixture<PersonSearch>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PersonSearch],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PersonSearch);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
