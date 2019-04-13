import {DataSource} from '@angular/cdk/table';
import {Transaction} from '../../models/transaction.model';
import {BehaviorSubject, merge, Observable, of} from 'rxjs';
import {MatPaginator, MatSort} from '@angular/material';
import {NotificationService} from '../../services';
import {CollectionViewer} from '@angular/cdk/collections';
import {catchError, finalize, map} from 'rxjs/operators';
import {TransactionsService} from '../../services/transactions.service';
import {ApiResponse} from '../../models/api-response';


export class TransactionDataSource extends DataSource<Transaction> {

  private transactionSubject = new BehaviorSubject<Transaction[]>([]);
  public transactions$ = this.transactionSubject.asObservable();
  public totalItems: number;
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  public filterSubject = new BehaviorSubject<string>('');
  public filter$ = this.filterSubject.asObservable();

  constructor(
    private transactionService: TransactionsService,
    private paginator: MatPaginator,
    private sort: MatSort,
    private notificationService: NotificationService) {
    super();
    this.filter$.subscribe(() => this.paginator.pageIndex = 0);
  }

  connect(): Observable<Transaction[]> {
    const dataMutattion = [this.transactions$, this.paginator.page, this.sort.sortChange, this.filter$];
    return merge(...dataMutattion)
      .pipe(map(() => {
        return this.getTransactionsPaged(this.getSortedData([...this.transactionSubject.value]));
      }));
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.transactionSubject.complete();
    this.loadingSubject.complete();
  }

  loadTransactions() {
    this.loadingSubject.next(true);
    this.transactionService.getAll()
      .pipe(
        catchError(() => of(this.notificationService.error('Server error.'))),
        finalize(() => this.loadingSubject.next(false))
      )
      .subscribe((response: ApiResponse<Transaction[]>) => {
        this.transactionSubject.next(response.result);
        this.totalItems = (this.transactionSubject.getValue()).length;
      });
  }

  getTransactionsPaged(transactionList: Transaction[]) {
    if (this.filterSubject.getValue() === '') {
      const startIndex = this.paginator.pageIndex * this.paginator.pageSize;
      return transactionList.splice(startIndex, this.paginator.pageSize);
    } else {
      return [...transactionList].slice().filter((transaction: Transaction) => {
        const searchStr = transaction.user.toLowerCase();
        return searchStr.indexOf(this.filterSubject.getValue().toLowerCase()) !== -1;
      });
    }
  }

  getSortedData(data: Transaction []) {
    if (!this.sort.active || this.sort.direction === '') {
      return data;
    }
    return data.sort((a, b) => {
      const isAsc = this.sort.direction === 'asc';
      switch (this.sort.active) {
        case 'id':
          return compare(a.id, b.id, isAsc);
        case 'transactionStatus':
          return compare(a.transactionStatus, b.transactionStatus, isAsc);
        case 'transactionType':
          return compare(a.transactionType, b.transactionType, isAsc);
        case 'cashMoved':
          return compare(a.cashMoved, b.cashMoved, isAsc);
        case 'creationTime':
          return compare(a.creationTime, b.creationTime, isAsc);
        case 'completeTime':
          return compare(a.completeTime, b.completeTime, isAsc);
        default:
          return 0;
      }
    });
  }
  handleDataChange(transactionResponse: ApiResponse<Transaction>) {
    const transactionList: Transaction [] = this.transactionSubject.value;
    const index = transactionList.findIndex(item => item.id === transactionResponse.result.id);
    transactionList[index] = transactionResponse.result;
    this.transactionSubject.next(transactionList);
  }

  handleDeleteDataChange(transaction: Transaction) {
    const transactionList: Transaction [] = this.transactionSubject.value;
    const index = transactionList.findIndex(item => item.id === transaction.id);
    transactionList.splice(index, 1);
    this.transactionSubject.next(transactionList);
    this.totalItems = (this.transactionSubject.getValue()).length;
  }

  handleCreateDataChange(transactionResponse: ApiResponse<Transaction>) {
    const transactionList: Transaction [] = this.transactionSubject.value;
    transactionList.push(transactionResponse.result);
    this.transactionSubject.next(transactionList);
    this.totalItems = (this.transactionSubject.getValue()).length;
  }


}

function compare(a: any, b: any, isAsc) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
