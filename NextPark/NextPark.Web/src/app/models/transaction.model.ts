import {BaseModel} from './base.model';
import {TransactionType} from './enums/transaction-type';
import {TransactionStatus} from './enums/transaction-status';

export class Transaction extends BaseModel {
   transactionId: string ;
   creationDate: Date ;
   completationDate: Date ;
   status: TransactionStatus ;
   transactionStatus: string;
   type: TransactionType ;
   transactionType: string;
   cashMoved: number ;
   user: string ;
   userId: number ;
   purchaseId: string ;
   purchaseToken: string ;
   purchaseState: string ;
   creationTime: string;
   completeTime: string;
}
