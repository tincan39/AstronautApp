import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-overlay',
  imports: [MatProgressSpinnerModule],
  templateUrl: './loading-overlay.html',
  styleUrl: './loading-overlay.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class.is-loading]': 'loading()' }
})
export class LoadingOverlay {
  loading = input(false);
}
