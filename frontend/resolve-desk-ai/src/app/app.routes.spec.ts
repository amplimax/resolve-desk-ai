import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter, Router, TitleStrategy } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { routes } from './app.routes';
import { PageTitleStrategy } from './core/services/page-title-strategy';

describe('Public and demo routes', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideRouter(routes),
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: TitleStrategy, useClass: PageTitleStrategy },
      ],
    });
  });

  it('keeps home indexable and clears its metadata on demo and error routes', async () => {
    const harness = await RouterTestingHarness.create('/');
    expect(harness.routeNativeElement?.textContent).toContain('A clearer path');
    expect(document.querySelector('link[rel="canonical"]')?.getAttribute('href')).toBe(
      'https://resolvedeskai.anywaresolutions.eu/',
    );
    expect(document.querySelector('meta[name="robots"]')?.getAttribute('content')).toBe(
      'index, follow',
    );

    await harness.navigateByUrl('/tickets/new');
    expect(document.querySelector('meta[name="robots"]')?.getAttribute('content')).toBe(
      'noindex, follow',
    );
    expect(document.querySelector('link[rel="canonical"]')).toBeNull();
    expect(document.querySelector('meta[name="description"]')).toBeNull();

    await harness.navigateByUrl('/unknown-page');
    expect(harness.routeNativeElement?.textContent).toContain('Page not found');
    expect(TestBed.inject(Router).url).toBe('/unknown-page');
    expect(document.title).toBe('Page not found | ResolveDesk AI');

    await harness.navigateByUrl('/');
    expect(document.querySelector('meta[name="robots"]')?.getAttribute('content')).toBe(
      'index, follow',
    );
    expect(document.querySelectorAll('link[rel="canonical"]')).toHaveLength(1);
  });

  it('shows an error for malformed ticket addresses', async () => {
    const harness = await RouterTestingHarness.create('/tickets/not-a-guid');
    expect(harness.routeNativeElement?.textContent).toContain('Page not found');
    expect(document.querySelector('meta[name="robots"]')?.getAttribute('content')).toBe(
      'noindex, follow',
    );
  });
});
