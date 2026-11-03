import { defer, finalize, MonoTypeOperatorFunction } from 'rxjs';
import { LoadingService } from './loading-service';

export function trackLoading<T>(loading: LoadingService): MonoTypeOperatorFunction<T> {
  return source => defer(() => {
    loading.increment();
    return source.pipe(finalize(() => loading.decrement()));
  });
}
