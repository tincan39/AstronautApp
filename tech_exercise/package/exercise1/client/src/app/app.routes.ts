import { Routes } from '@angular/router';
import { PersonSearch } from './person-search/person-search';

export const routes: Routes = [
    { path:'search', component: PersonSearch },
    { path: '', redirectTo: '/search', pathMatch: 'full' }, 
];
