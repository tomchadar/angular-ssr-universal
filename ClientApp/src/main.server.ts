import 'rxjs-compat/add/operator/first';
import 'zone.js/dist/zone-node';
import 'reflect-metadata';
import { INITIAL_CONFIG, renderModule, renderModuleFactory } from '@angular/platform-server';
import { APP_BASE_HREF } from '@angular/common';
import { enableProdMode, StaticProvider, NgZone, Inject } from '@angular/core';
import { createServerRenderer, BootFuncParams } from 'aspnet-prerendering'
import { environment } from './environments/environment';
import { AppModule } from './app/app.module';


if (environment.production) {
  enableProdMode();
}

export default createServerRenderer(params => {
  const { AppServerModule, AppServerModuleNgFactory, LAZY_MODULE_MAP } = (module as any).exports;

  const providers: StaticProvider[] = [
    { provide: APP_BASE_HREF, useValue: params.baseUrl },
    { provide: 'BOOT_PARAMS', useValue: params },
    { provide: 'SERVERSIDE', useValue: true },
    { provide: 'MESSAGE', useValue: params.data.message },
  ];

  const options = {
    document: params.data.originalHtml,
    url: params.url,
    extraProviders: providers
  };

  // Bypass ssr api call cert warnings in development
  process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";

  const renderPromise = AppServerModuleNgFactory
    ? /* AoT */ renderModuleFactory(AppServerModuleNgFactory, options)
    : /* dev */ renderModule(AppServerModule, options);

  return renderPromise.then(html => ({ html }));
});



export { AppServerModule } from './app/app.server.module';
export { renderModule, renderModuleFactory } from '@angular/platform-server';

