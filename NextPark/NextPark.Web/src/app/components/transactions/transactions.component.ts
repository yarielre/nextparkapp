import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {MatDialog, MatDialogConfig, MatPaginator, MatSort} from '@angular/material';
import {TransactionDataSource} from '../../_helpers/data-sources/transaction.data.source';
import {TransactionsService} from '../../services/transactions.service';
import {NotificationService} from '../../services';
import {fromEvent} from 'rxjs';
import {debounceTime, distinctUntilChanged} from 'rxjs/operators';
import {Transaction} from '../../models/transaction.model';
import {TransactionFormComponent} from '../_forms/transaction-form/transaction-form.component';
import {TransactionDeleteConfirmDialogComponent} from '../_shared/delete-confirm-dialog/transaction-delete-confirm-dialog.component';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.scss']
})
export class TransactionsComponent implements OnInit {

  displayedColumns: string[] = ['id',
    'user',
    'transactionStatus',
    'transactionType',
    'cashMoved',
    'creationTime',
    'completeTime',
    'actions',
    'create'];

  transactionDataSource: TransactionDataSource;
  @ViewChild(MatPaginator) paginate: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('filter') filter: ElementRef;

  constructor(private transactionsService: TransactionsService,
              private notifService: NotificationService,
              private dialog: MatDialog) {
  }

  ngOnInit() {
    fromEvent(this.filter.nativeElement, 'keyup')
      .pipe(
        debounceTime(150),
        distinctUntilChanged())
      .subscribe(() => {
        this.transactionDataSource.filterSubject.next(this.filter.nativeElement.value);
      });
    this.transactionDataSource = new TransactionDataSource(this.transactionsService, this.paginate, this.sort, this.notifService);
    this.transactionDataSource.loadTransactions();
  }

  onEdit(transactionRow: Transaction) {
    this.transactionsService.fillForm(transactionRow);
    this.openEditDialog();
  }

  onCreate() {
    this.transactionsService.initForm();
    const dialogConfig: MatDialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.width = '60%';
    this.dialog.open(TransactionFormComponent, dialogConfig)
      .afterClosed()
      .subscribe(transactionCreatedResult => {
        if (transactionCreatedResult !== undefined && transactionCreatedResult.isCreated) {
          if (transactionCreatedResult.payload !== undefined) {
            this.transactionDataSource.handleCreateDataChange(transactionCreatedResult.payload);
            this.notifService.success('Transaction created.');
          } else {
            this.notifService.success('Server error');
          }
        }
      });
  }

  onDelete(transactionRow: Transaction) {
    this.openDeleteDialog(transactionRow);
  }

  openEditDialog() {
    const dialogConfig: MatDialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.width = '60%';
    this.dialog.open(TransactionFormComponent, dialogConfig)
      .afterClosed()
      .subscribe(transactionEditedResult => {
          if (transactionEditedResult !== undefined && transactionEditedResult.isUpdated) {
            if (transactionEditedResult.payload !== undefined) {
              this.transactionDataSource.handleDataChange(transactionEditedResult.payload);
              this.notifService.success('Transaction updated.');
            } else {
              this.notifService.success('Server error');
            }
          }
        }
      );
  }

  openDeleteDialog(transaction: Transaction) {
    const dialogConfig: MatDialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.width = '40%';
    dialogConfig.data = {
      title: 'Delete User',
      text: `Are you sure you want to delete a transaction of user <em>${transaction.user}.</em>`,
      payload: transaction
    };

    this.dialog.open(TransactionDeleteConfirmDialogComponent, dialogConfig)
      .afterClosed()
      .subscribe(deleteDialogdata => {
        if (deleteDialogdata !== undefined && deleteDialogdata.isOnDelete) {
          this.transactionDataSource.handleDeleteDataChange(deleteDialogdata.payload);
          this.notifService.success('Transaction deleted.');
        }
      });

  }

  public IsFiltered(): boolean {
    if (this.filter.nativeElement.value === '') {
      return false;
    } else {
      return true;
    }
  }

  clearFilter() {
    this.filter.nativeElement.value = '';
    this.transactionDataSource.filterSubject.next(this.filter.nativeElement.value);
  }

}
