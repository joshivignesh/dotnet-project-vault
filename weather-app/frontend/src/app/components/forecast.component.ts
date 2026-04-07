import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { WeatherService } from '../services/weather.service';
import { ForecastItem } from '../models/weather.model';

@Component({
  selector: 'app-forecast',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
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
                {{ svc.convert(item.temperature) }}°{{ svc.unit() }}
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
