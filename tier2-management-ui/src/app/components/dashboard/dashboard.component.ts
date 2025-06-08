import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { RouterModule } from '@angular/router';

interface DashboardStats {
  totalFiles: number;
  lockedFiles: number;
  deletedItems: number;
  errorQueueCount: number;
  activeUsers: number;
  lastSyncTime: Date;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatGridListModule,
    MatProgressBarModule,
    RouterModule
  ],
  template: `
    <div class="dashboard-container">
      <h1 class="dashboard-title">לוח בקרה - מערכת ניהול רובד 2</h1>
      
      <mat-grid-list cols="4" rowHeight="200px" gutterSize="16px" class="dashboard-grid">
        <!-- Files Overview -->
        <mat-grid-tile>
          <mat-card class="dashboard-card">
            <mat-card-header>
              <mat-icon mat-card-avatar class="card-icon files-icon">folder</mat-icon>
              <mat-card-title>קבצים</mat-card-title>
              <mat-card-subtitle>סקירת קבצים במערכת</mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              <div class="stat-number">{{ stats.totalFiles | number }}</div>
              <div class="stat-label">סה"כ קבצים</div>
              <div class="stat-detail">
                <span class="locked-files">{{ stats.lockedFiles }}</span> נעולים
              </div>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button routerLink="/files" color="primary">
                <mat-icon>arrow_back</mat-icon>
                ניהול קבצים
              </button>
            </mat-card-actions>
          </mat-card>
        </mat-grid-tile>

        <!-- Deleted Items -->
        <mat-grid-tile>
          <mat-card class="dashboard-card">
            <mat-card-header>
              <mat-icon mat-card-avatar class="card-icon deleted-icon">delete</mat-icon>
              <mat-card-title>פריטים מחוקים</mat-card-title>
              <mat-card-subtitle>פריטים הממתינים לשחזור</mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              <div class="stat-number">{{ stats.deletedItems | number }}</div>
              <div class="stat-label">פריטים מחוקים</div>
              <div class="stat-detail">זמינים לשחזור</div>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button routerLink="/files" color="primary">
                <mat-icon>restore</mat-icon>
                שחזור פריטים
              </button>
            </mat-card-actions>
          </mat-card>
        </mat-grid-tile>

        <!-- Queue Status -->
        <mat-grid-tile>
          <mat-card class="dashboard-card" [class.error-card]="stats.errorQueueCount > 0">
            <mat-card-header>
              <mat-icon mat-card-avatar class="card-icon queue-icon">queue</mat-icon>
              <mat-card-title>תור שגיאות</mat-card-title>
              <mat-card-subtitle>הודעות בתור השגיאות</mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              <div class="stat-number" [class.error-number]="stats.errorQueueCount > 0">
                {{ stats.errorQueueCount | number }}
              </div>
              <div class="stat-label">הודעות שגיאה</div>
              <div class="stat-detail" *ngIf="stats.errorQueueCount > 0">
                דורש טיפול מיידי
              </div>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button routerLink="/queues" color="warn" *ngIf="stats.errorQueueCount > 0">
                <mat-icon>warning</mat-icon>
                טיפול בשגיאות
              </button>
              <button mat-button routerLink="/queues" color="primary" *ngIf="stats.errorQueueCount === 0">
                <mat-icon>check_circle</mat-icon>
                ניהול תורים
              </button>
            </mat-card-actions>
          </mat-card>
        </mat-grid-tile>

        <!-- Users Status -->
        <mat-grid-tile>
          <mat-card class="dashboard-card">
            <mat-card-header>
              <mat-icon mat-card-avatar class="card-icon users-icon">people</mat-icon>
              <mat-card-title>משתמשים</mat-card-title>
              <mat-card-subtitle>משתמשים פעילים במערכת</mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              <div class="stat-number">{{ stats.activeUsers | number }}</div>
              <div class="stat-label">משתמשים פעילים</div>
              <div class="stat-detail">
                סנכרון אחרון: {{ stats.lastSyncTime | date:'short':'he-IL' }}
              </div>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button routerLink="/users" color="primary">
                <mat-icon>manage_accounts</mat-icon>
                ניהול משתמשים
              </button>
            </mat-card-actions>
          </mat-card>
        </mat-grid-tile>
      </mat-grid-list>

      <!-- System Status -->
      <mat-card class="system-status-card margin-top">
        <mat-card-header>
          <mat-icon mat-card-avatar class="card-icon system-icon">computer</mat-icon>
          <mat-card-title>סטטוס המערכת</mat-card-title>
          <mat-card-subtitle>מצב כללי של המערכת</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <div class="status-grid">
            <div class="status-item">
              <mat-icon class="status-success">check_circle</mat-icon>
              <span>שרת API</span>
              <span class="status-text">פעיל</span>
            </div>
            <div class="status-item">
              <mat-icon class="status-success">check_circle</mat-icon>
              <span>שרת שיתוף</span>
              <span class="status-text">פעיל</span>
            </div>
            <div class="status-item">
              <mat-icon class="status-success">check_circle</mat-icon>
              <span>RabbitMQ</span>
              <span class="status-text">פעיל</span>
            </div>
            <div class="status-item">
              <mat-icon class="status-success">check_circle</mat-icon>
              <span>Active Directory</span>
              <span class="status-text">מחובר</span>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <!-- Quick Actions -->
      <mat-card class="quick-actions-card margin-top">
        <mat-card-header>
          <mat-icon mat-card-avatar class="card-icon actions-icon">flash_on</mat-icon>
          <mat-card-title>פעולות מהירות</mat-card-title>
          <mat-card-subtitle>פעולות נפוצות במערכת</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <div class="quick-actions">
            <button mat-raised-button color="primary" routerLink="/files">
              <mat-icon>lock_open</mat-icon>
              שחרור נעילות קבצים
            </button>
            <button mat-raised-button color="accent" routerLink="/files">
              <mat-icon>restore</mat-icon>
              שחזור גרסאות קבצים
            </button>
            <button mat-raised-button color="warn" routerLink="/queues">
              <mat-icon>transfer_within_a_station</mat-icon>
              העברת הודעות בתורים
            </button>
            <button mat-raised-button routerLink="/users">
              <mat-icon>security</mat-icon>
              בדיקת הרשאות משתמש
            </button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .dashboard-title {
      text-align: center;
      margin-bottom: 30px;
      color: #333;
      font-weight: 500;
    }

    .dashboard-grid {
      margin-bottom: 20px;
    }

    .dashboard-card {
      width: 100%;
      height: 100%;
      display: flex;
      flex-direction: column;
      justify-content: space-between;
    }

    .card-icon {
      font-size: 40px;
      width: 40px;
      height: 40px;
    }

    .files-icon {
      color: #2196f3;
    }

    .deleted-icon {
      color: #ff9800;
    }

    .queue-icon {
      color: #4caf50;
    }

    .users-icon {
      color: #9c27b0;
    }

    .system-icon {
      color: #607d8b;
    }

    .actions-icon {
      color: #ff5722;
    }

    .stat-number {
      font-size: 2.5em;
      font-weight: bold;
      text-align: center;
      margin: 10px 0;
    }

    .stat-label {
      text-align: center;
      color: #666;
      font-size: 0.9em;
    }

    .stat-detail {
      text-align: center;
      color: #999;
      font-size: 0.8em;
      margin-top: 5px;
    }

    .locked-files {
      color: #ff9800;
      font-weight: bold;
    }

    .error-card {
      border-right: 4px solid #f44336;
    }

    .error-number {
      color: #f44336;
    }

    .system-status-card,
    .quick-actions-card {
      width: 100%;
    }

    .status-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
      margin-top: 16px;
    }

    .status-item {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 8px;
      border-radius: 4px;
      background-color: #f5f5f5;
    }

    .status-text {
      margin-right: auto;
      font-weight: 500;
    }

    .quick-actions {
      display: flex;
      flex-wrap: wrap;
      gap: 16px;
      justify-content: center;
      margin-top: 16px;
    }

    .quick-actions button {
      min-width: 200px;
    }

    @media (max-width: 768px) {
      .dashboard-grid {
        grid-template-columns: repeat(2, 1fr) !important;
      }
      
      .quick-actions {
        flex-direction: column;
        align-items: center;
      }
    }

    @media (max-width: 480px) {
      .dashboard-grid {
        grid-template-columns: 1fr !important;
      }
    }
  `]
})
export class DashboardComponent implements OnInit {
  stats: DashboardStats = {
    totalFiles: 0,
    lockedFiles: 0,
    deletedItems: 0,
    errorQueueCount: 0,
    activeUsers: 0,
    lastSyncTime: new Date()
  };

  ngOnInit() {
    this.loadDashboardStats();
  }

  private loadDashboardStats() {
    // Mock data - in real implementation, this would call the API
    this.stats = {
      totalFiles: 15420,
      lockedFiles: 23,
      deletedItems: 8,
      errorQueueCount: 2,
      activeUsers: 147,
      lastSyncTime: new Date(Date.now() - 3600000) // 1 hour ago
    };
  }
} 