import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { NotFound } from '../../shared/not-found/not-found';

@Component({
  selector: 'app-page-not-found',
  imports: [NotFound, MatButtonModule, MatIconModule, RouterLink],
  templateUrl: './page-not-found.html',
  styleUrl: './page-not-found.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PageNotFound {}
