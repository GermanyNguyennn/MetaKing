import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'MetaKing Admin',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:5000/',
    redirectUri: baseUrl,
    clientId: 'MetaKing_Admin',
    dummyClientSecret: '23092003',
    responseType: 'code',
    scope: 'offline_access MetaKing.Admin',
    requireHttps: true
  },
  apis: {
    default: {
      url: 'https://localhost:5001',
      rootNamespace: 'MetaKing.Admin',
    },
  },
} as Environment;
