import { DOCUMENT, inject, Injectable } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { RouterStateSnapshot, TitleStrategy } from '@angular/router';

@Injectable()
export class PageTitleStrategy extends TitleStrategy {
  private readonly document = inject(DOCUMENT);
  private readonly meta = inject(Meta);
  private readonly title = inject(Title);

  override updateTitle(snapshot: RouterStateSnapshot): void {
    this.title.setTitle(this.buildTitle(snapshot) ?? 'ResolveDesk AI');

    let route = snapshot.root;
    while (route.firstChild) route = route.firstChild;
    const indexable = route.data['indexable'] === true;

    this.meta.updateTag({
      name: 'robots',
      content: indexable ? 'index, follow' : 'noindex, follow',
    });
    this.document.querySelector('link[rel="canonical"]')?.remove();

    if (indexable) {
      this.meta.updateTag({
        name: 'description',
        content:
          'Manage support tickets with ResolveDesk AI. Explore AI-assisted classification, summaries and suggested replies in an interactive Angular and ASP.NET Core demo.',
      });
      const canonical = this.document.createElement('link');
      canonical.rel = 'canonical';
      canonical.href = 'https://resolvedeskai.anywaresolutions.eu/';
      this.document.head.appendChild(canonical);
    } else {
      this.meta.removeTag('name="description"');
    }
  }
}
