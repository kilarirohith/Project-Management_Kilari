// src/app/interceptors/auth.interceptor.ts
import { HttpInterceptorFn } from '@angular/common/http';

const isBrowser = () =>
  typeof window !== 'undefined' && typeof localStorage !== 'undefined';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (!isBrowser()) {
    return next(req);
  }

  const token = localStorage.getItem('token');

  if (token) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(authReq);
  }

  return next(req);
};
