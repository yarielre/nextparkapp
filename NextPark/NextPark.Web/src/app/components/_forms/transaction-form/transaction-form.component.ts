import { Component, OnInit } from '@angular/core';
import {TransactionsService} from '../../../services/transactions.service';
import {MatDialogRef} from '@angular/material';

@Component({
  selector: 'app-transaction-form',
  templateUrl: './transaction-form.component.html',
  styleUrls: ['./transaction-form.component.scss']
})
export class TransactionFormComponent implements OnInit {

  status: { key: string, value: number }[] = [
    {key: 'Pending', value: 0},
    {key: 'Completed', value: 1},
    {key: 'Rejected', value: 2}
  ];

  type: { key: string, value: number }[] = [
    {key: 'FeedTransaction', value: 0},
    {key: 'RentTransaction', value: 1},
    {key: 'WithdrawalTransaction', value: 2},
    {key: 'IncreaseBalanceTransaction', value: 3}
  ];

  constructor(public transactionService: TransactionsService,
              private dialogRef: MatDialogRef<TransactionFormComponent>) {
  }

  ngOnInit() {
  }

  onSave(transaction: any) {
    if (transaction.id > 0) {
      this.transactionService.update(transaction).subscribe(transactionEdited => {
        this.onClose({isUpdated: true, payload: transactionEdited}
        );
      }, error => {
        this.dialogRef.close({isUpdated: true});
      });
    } else {
      this.transactionService.add(transaction).subscribe(transactionCreated => {
        this.onClose({isCreated: true, payload: transactionCreated});
      }, error => {
        this.dialogRef.close({isCreated: true});
      });
    }

  }

  onClose(dialogResult) {
    this.dialogRef.close(dialogResult);
  }


}
