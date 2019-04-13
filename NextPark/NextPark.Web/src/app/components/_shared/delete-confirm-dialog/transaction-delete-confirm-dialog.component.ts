import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';
import {TransactionsService} from '../../../services/transactions.service';
import {NotificationService} from '../../../services';

@Component({
  selector: 'app-transaction-delete-confirm-dialog',
  templateUrl: './delete-confirm-dialog.component.html',
  styleUrls: ['./delete-confirm-dialog.component.scss']
})
export class TransactionDeleteConfirmDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              private matDialogRef: MatDialogRef<TransactionDeleteConfirmDialogComponent>,
              private transactionsService: TransactionsService,
              private notifcationService: NotificationService) {

  }


  ngOnInit() {

  }

  onDelete(): void {
    this.transactionsService.delete(this.data.payload.id)
      .subscribe(user => {
        this.matDialogRef.close({isOnDelete: true, payload: this.data.payload});
      }, error => {
        this.notifcationService.error('Server error');
        this.matDialogRef.close();
      });
  }

  onCancel(): void {
    this.matDialogRef.close({isOnDelete: false});
  }

}
