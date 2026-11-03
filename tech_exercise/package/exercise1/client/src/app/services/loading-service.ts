import { computed, Injectable, signal } from '@angular/core';

@Injectable()
export class LoadingService {
  private readonly count = signal(0);
  readonly isLoading = computed(() => this.count() > 0);

  increment(): void {
    this.count.update(c => c + 1);
  }

  decrement(): void {
    this.count.update(c => Math.max(0, c - 1));
  }
}
