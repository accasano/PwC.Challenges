import {
  HashLocationStrategy,
  LocationStrategy,
} from '@angular/common';
import {
  provideHttpClient,
  withFetch,
} from '@angular/common/http';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';

import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';
import { AuthService } from './app/features/auth/services/auth.service';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(withFetch()),
    AuthService,
    { provide: LocationStrategy, useClass: HashLocationStrategy }
  ],
}).catch((err) => console.error(err));
