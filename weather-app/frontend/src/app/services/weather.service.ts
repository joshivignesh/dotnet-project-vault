import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { rxResource } from '@angular/core/rxjs-interop';
import { WeatherSummary, ForecastSummary } from '../models/weather.model';

@Injectable({ providedIn: 'root' })
export class WeatherService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5000';
  private readonly maxHistory = 5;

  readonly city = signal('');
  readonly history = signal<string[]>([]);

  readonly weather = rxResource<WeatherSummary, string>({
    request: () => this.city(),
    loader: ({ request: city }) =>
      this.http.get<WeatherSummary>(`${this.apiUrl}/weather/${city}`),
  });

  readonly forecast = rxResource<ForecastSummary, string>({
    request: () => this.city(),
    loader: ({ request: city }) =>
      this.http.get<ForecastSummary>(`${this.apiUrl}/weather/${city}/forecast`),
  });

  readonly isLoading = computed(() =>
    this.weather.isLoading() || this.forecast.isLoading()
  );

  readonly hasError = computed(() =>
    !!this.weather.error() || !!this.forecast.error()
  );

  search(city: string) {
    const trimmed = city.trim();
    if (!trimmed) return;
    this.city.set(trimmed);
    this.addToHistory(trimmed);
  }

  private addToHistory(city: string) {
    this.history.update(prev => {
      const filtered = prev.filter(c => c.toLowerCase() !== city.toLowerCase());
      return [city, ...filtered].slice(0, this.maxHistory);
    });
  }
}
