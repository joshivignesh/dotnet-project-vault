import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { WeatherService } from '../services/weather.service';

@Component({
  selector: 'app-weather-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (svc.weather.isLoading()) {
      <div class="card loading">Fetching weather...</div>
    }

    @if (svc.weather.error()) {
      <div class="card error">City not found. Try another name.</div>
    }

    @if (svc.weather.value(); as w) {
      <div class="card">
        <div class="card-header">
          <h2>{{ w.city }}, {{ w.country }}</h2>
          <img
            [src]="'https://openweathermap.org/img/wn/' + w.icon + '@2x.png'"
            [alt]="w.description"
          />
        </div>

        <div class="temp">{{ svc.convert(w.temperature) }}°{{ svc.unit() }}</div>
        <div class="condition">{{ w.description }}</div>
        <div class="feels-like">
          Feels like {{ svc.convert(w.feelsLike) }}°{{ svc.unit() }} ·
          Low {{ svc.convert(w.tempMin) }}° · High {{ svc.convert(w.tempMax) }}°
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
