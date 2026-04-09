import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { DecimalPipe, DatePipe } from '@angular/common';
import { WeatherService } from '../services/weather.service';
import { SkeletonComponent } from './skeleton.component';

@Component({
  selector: 'app-weather-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DecimalPipe, DatePipe, SkeletonComponent],
  template: `
    @if (svc.weather.isLoading()) {
      <div class="card">
        <div class="card-header">
          <app-skeleton width="160px" height="28px" />
          <app-skeleton width="64px" height="64px" radius="8px" />
        </div>
        <app-skeleton width="120px" height="56px" radius="8px" />
        <app-skeleton width="180px" height="16px" />
        <app-skeleton width="140px" height="14px" />
        <div class="skeleton-row">
          <app-skeleton width="60px" height="14px" />
          <app-skeleton width="60px" height="14px" />
          <app-skeleton width="60px" height="14px" />
        </div>
      </div>
    }

    @if (svc.weather.error()) {
      <div class="error">City not found. Try another name.</div>
    }

    @if (svc.weather.value(); as w) {
      <div class="card">
        <div class="card-header">
          <h2>{{ w.city }}, {{ w.country }}</h2>
          <div class="card-header-right">
            <button class="unit-toggle" (click)="svc.toggleUnit()">
              {{ svc.unit() === 'C' ? '°C → °F' : '°F → °C' }}
            </button>
            <img
              [src]="'https://openweathermap.org/img/wn/' + w.icon + '@2x.png'"
              [alt]="w.description"
            />
          </div>
        </div>

        <div class="temp">{{ svc.convertTemp(w.temperature) }}°{{ svc.unit() }}</div>
        <div class="condition">{{ w.description }}</div>
        <div class="feels-like">
          Feels like {{ svc.convertTemp(w.feelsLike) }}°{{ svc.unit() }}
          · Low {{ svc.convertTemp(w.tempMin) }}°
          · High {{ svc.convertTemp(w.tempMax) }}°
        </div>

        <div class="details">
          <span>💧 {{ w.humidity }}%</span>
          <span>💨 {{ w.windSpeed }} m/s</span>
          <span>👁 {{ w.visibility / 1000 | number:'1.0-1' }} km</span>
        </div>

        <div class="sun-times">
          <span>🌅 {{ w.sunrise | date:'HH:mm' }}</span>
          <span>🌇 {{ w.sunset | date:'HH:mm' }}</span>
        </div>
      </div>
    }
  `,
})
export class WeatherCardComponent {
  readonly svc = inject(WeatherService);
}
