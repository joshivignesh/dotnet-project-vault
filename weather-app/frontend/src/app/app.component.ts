import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { WeatherService } from './services/weather.service';
import { WeatherCardComponent } from './components/weather-card.component';
import { ForecastComponent } from './components/forecast.component';
import { SearchHistoryComponent } from './components/search-history.component';

@Component({
  selector: 'app-root',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [WeatherCardComponent, ForecastComponent, SearchHistoryComponent],
  template: `
    <div class="container">
      <div class="header">
        <h1>🌤 Weather</h1>
        <button class="unit-toggle" (click)="svc.toggleUnit()">
          °{{ svc.unit() === 'C' ? 'F' : 'C' }}
        </button>
      </div>

      <div class="search">
        <input
          type="text"
          placeholder="Enter city name..."
          [value]="input()"
          (input)="input.set($any($event.target).value)"
          (keydown.enter)="search()"
        />
        <button (click)="search()" [disabled]="!input().trim()">
          Search
        </button>
      </div>

      <app-search-history />

      @if (svc.city()) {
        <app-weather-card />
        <app-forecast />
      }
    </div>
  `,
})
export class AppComponent {
  readonly svc = inject(WeatherService);
  readonly input = signal('');

  search() {
    const city = this.input().trim();
    if (!city) return;
    this.svc.search(city);
    this.input.set('');
  }
}
