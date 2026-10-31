import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonSearch } from './person-search';

describe('PersonSearch', () => {
  let component: PersonSearch;
  let fixture: ComponentFixture<PersonSearch>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PersonSearch]
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
