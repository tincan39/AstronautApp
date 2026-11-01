import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

export type StatusType = 'error' | 'success';

@Component({
  selector: 'app-status-message',
  imports: [MatIconModule],
  templateUrl: './status-message.html',
  styleUrl: './status-message.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StatusMessage {
  message = input.required<string>();
  status = input.required<StatusType>();
}
