import { Routes } from '@angular/router';
import { PersonSearch } from './pages/person-search/person-search';
import { PageNotFound } from './pages/page-not-found/page-not-found';
import { PersonPage } from './pages/person/person-page';

export const routes: Routes = [
    { path:'search', component: PersonSearch },
    { path: 'person', component: PersonPage },
    { path: 'person/:name', component: PersonPage },
    { path: '', redirectTo: '/search', pathMatch: 'full' },
    { path: '**', component: PageNotFound },
];
