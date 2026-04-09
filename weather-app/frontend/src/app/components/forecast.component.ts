import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { DatePipe } from '@angular/common';
import { WeatherService } from '../services/weather.service';
import { SkeletonComponent } from './skeleton.component';
import { ForecastItem } from '../models/weather.model';

@Component({
  selector: 'app-forecast',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, SkeletonComponent],
  template: `
    @if (svc.forecast.isLoading()) {
      <div class="forecast">
        <app-skeleton width="120px" height="16px" />
        <div class="forecast-grid" style="margin-top: 16px">
          @for (n of [1,2,3,4,5]; track n) {
            <div class="forecast-item">
              <app-skeleton width="32px" height="12px" radius="4px" />
              <app-skeleton width="36px" height="36px" radius="50%" />
              <app-skeleton width="40px" height="18px" radius="4px" />
              <app-skeleton width="48px" height="10px" radius="4px" />
            </div>
          }
        </div>
      </div>
    }

    @if (svc.forecast.value(); as f) {
      <div class="forecast">
        <h3>5-Day Forecast</h3>
        <div class="forecast-grid">
          @for (item of dailyItems(f.items); track item.dateTime) {
            <div class="forecast-item">
              <div class="day">{{ item.dateTime | date:'EEE' }}</div>
              <img
                [src]="'https://openweathermap.org/img/wn/' + item.icon + '.png'"
                [alt]="item.description"
              />
              <div class="forecast-temp">
                {{ svc.convertTemp(item.temperature) }}°{{ svc.unit() }}
              </div>
              <div class="forecast-desc">{{ item.condition }}</div>
            </div>
          }
        </div>
      </div>
    }
  `,
})
export class ForecastComponent {
  readonly svc = inject(WeatherService);

  dailyItems(items: ForecastItem[]): ForecastItem[] {
    const seen = new Set<string>();
    return items.filter(item => {
      const day = new Date(item.dateTime).toDateString();
      if (seen.has(day)) return false;
      seen.add(day);
      return true;
    }).slice(0, 5);
  }
}
