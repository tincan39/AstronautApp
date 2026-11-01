import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs';

@Component({
  selector: 'app-person',
  imports: [],
  templateUrl: './person-page.html',
  styleUrl: './person-page.css',
})
export class PersonPage {
  private route = inject(ActivatedRoute);
  name = toSignal(this.route.paramMap.pipe(map(params => params.get('name'))), { initialValue: null });
}
