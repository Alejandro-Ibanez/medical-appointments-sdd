import { HttpInterceptorFn } from '@angular/common/http';

const TOKEN_STORAGE_KEY = 'auth_token';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  let token: string | null = null;

  try {
    token = localStorage.getItem(TOKEN_STORAGE_KEY);
  } catch {
    token = null;
  }

  if (!token) {
    return next(req);
  }

  const authReq = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
  });

  return next(authReq);
};
