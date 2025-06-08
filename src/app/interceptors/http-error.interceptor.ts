import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private snackBar: MatSnackBar) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'אירעה שגיאה בלתי צפויה'; // Unexpected error occurred
        
        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMessage = `שגיאת לקוח: ${error.error.message}`;
        } else {
          // Server-side error
          switch (error.status) {
            case 400:
              errorMessage = 'בקשה לא תקינה';
              break;
            case 401:
              errorMessage = 'נדרשת הזדהות';
              break;
            case 403:
              errorMessage = 'אין הרשאה לביצוע הפעולה';
              break;
            case 404:
              errorMessage = 'הפריט לא נמצא';
              break;
            case 500:
              errorMessage = 'שגיאת שרת פנימית';
              break;
            default:
              errorMessage = `שגיאת שרת: ${error.status}`;
          }
        }
        
        this.snackBar.open(errorMessage, 'סגור', {
          duration: 5000,
          direction: 'rtl',
          horizontalPosition: 'start',
          verticalPosition: 'bottom'
        });
        
        return throwError(error);
      })
    );
  }
} 