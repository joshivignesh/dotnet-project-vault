import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { WeatherService } from '../services/weather.service';

@Component({
  selector: 'app-search-history',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (svc.history().length > 0) {
      <div class="history">
        <span class="history-label">Recent:</span>
        @for (city of svc.history(); track city) {
          <button
            class="history-chip"
            [class.active]="city === svc.city()"
            (click)="svc.search(city)"
          >
            {{ city }}
          </button>
        }
      </div>
    }
  `,
})
export class SearchHistoryComponent {
  readonly svc = inject(WeatherService);
}
