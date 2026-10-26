
import {
  Component,
  OnInit,
  computed,
  inject,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface Transaction {
  sourceSystem: string;
  transactionId: number;
  locationCode: string;
  productName: string;
  amount: number;
  transactionTime: string;
  status: string;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly http = inject(HttpClient);

  readonly transactions = signal<Transaction[]>([]);
  readonly loading = signal(false);
  readonly importing = signal(false);
  readonly errorMessage = signal('');
  readonly successMessage = signal('');

  readonly totalCount = computed(() =>
    this.transactions().length
  );

  readonly activeCount = computed(() =>
    this.transactions().filter(
      transaction => transaction.status === 'Active'
    ).length
  );

  readonly revokedCount = computed(() =>
    this.transactions().filter(
      transaction => transaction.status === 'Revoked'
    ).length
  );

  readonly finalizedCount = computed(() =>
    this.transactions().filter(
      transaction => transaction.status === 'Finalized'
    ).length
  );

  ngOnInit(): void {
    this.loadTransactions();
  }

  loadTransactions(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.http.get<Transaction[]>('/api/transactions')
      .subscribe({
        next: (data) => {
          this.transactions.set(data);
          this.loading.set(false);
        },
        error: () => {
          this.errorMessage.set(
            'Unable to load transactions. Please check that the API is running.'
          );
          this.loading.set(false);
        }
      });
  }

  importTransactions(): void {
    if (this.importing()) {
      return;
    }

    this.importing.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.http.post('/api/import', {})
      .subscribe({
        next: () => {
          this.importing.set(false);
          this.successMessage.set(
            'Transaction import completed successfully.'
          );
          this.loadTransactions();
        },
        error: () => {
          this.importing.set(false);
          this.errorMessage.set(
            'Transaction import failed. Please try again.'
          );
        }
      });
  }
}
