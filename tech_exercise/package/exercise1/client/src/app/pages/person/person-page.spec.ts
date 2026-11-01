import { ComponentFixture, TestBed } from '@angular/core/testing';

import { provideRouter } from '@angular/router';

import { PersonPage } from './person-page';

describe('PersonPage', () => {
  let component: PersonPage;
  let fixture: ComponentFixture<PersonPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PersonPage],
      providers: [provideRouter([])]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PersonPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
