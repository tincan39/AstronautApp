import { Routes } from '@angular/router';
import { PersonSearch } from './person-search/person-search';
import { PageNotFound } from './page-not-found/page-not-found';

export const routes: Routes = [
    { path:'search', component: PersonSearch },
    { path: '', redirectTo: '/search', pathMatch: 'full' },
    { path: '**', component: PageNotFound },
];
