export interface WeatherSummary {
  city: string;
  country: string;
  temperature: number;
  feelsLike: number;
  tempMin: number;
  tempMax: number;
  humidity: number;
  condition: string;
  description: string;
  icon: string;
  windSpeed: number;
  visibility: number;
  sunrise: string;
  sunset: string;
  fetchedAt: string;
}

export interface ForecastSummary {
  city: string;
  country: string;
  items: ForecastItem[];
}

export interface ForecastItem {
  dateTime: string;
  temperature: number;
  condition: string;
  description: string;
  icon: string;
  windSpeed: number;
  humidity: number;
}
