import { HttpInterceptorFn } from "@angular/common/http";

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('auth_token');

  if (token) {
    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    })
    return next(cloned);
  }

  return next(req);
}
