import { NEXT_PARK_URL } from './../../../_helpers/constants/host.settings';
import { Component, OnInit, Inject } from '@angular/core';
import { EmailSubjectMessage } from 'src/app/models';
import { EmailSenderService } from 'src/app/services/email-sender.service';
import { NotificationService } from 'src/app/services/notification.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-email-sender',
  templateUrl: './email-sender.component.html',
  styleUrls: ['./email-sender.component.scss']
})
export class EmailSenderComponent implements OnInit {

  // baseUrl: string = NEXT_PARK_URL.user;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              private emailSenderService: EmailSenderService,
              // private dialogRef: MatDialogRef<EmailSenderFormComponent>,
              private notifService: NotificationService) {
  }

  ngOnInit() {
  }

  // sendEmail(emailBody: EmailSubjectMessage) {
  //   if (this.data !== undefined) {
  //     if (this.data.isForAllUser) {
  //       this.sendEmailToAllUsers(emailBody);
  //     } else {
  //       this.sendEmailToUser(emailBody);
  //     }
  //   }
  //   this.onClose();
  // }

  // sendEmailToAllUsers(emailBody: EmailSubjectMessage) {
  //   this.emailSenderService.sendEmailToAllUsers(`${this.baseUrl}/sendtoall`, emailBody)
  //     .subscribe(ok => {
  //         this.notifService.success('Email sent to all users.');
  //       },
  //       error => {
  //         this.notifService.error('Server error trying to send email.');
  //       }
  //     );
  // }

  // sendEmailToUser(emailBody: EmailSubjectMessage) {
  //   if (this.data !== undefined && this.data.payload !== null) {
  //     this.emailSenderService.sendEmailToUser(`${this.baseUrl}/sendtoone`, emailBody, this.data.payload)
  //       .subscribe(ok => {
  //           this.notifService.success('Email sent.');
  //         },
  //         error => {
  //           this.notifService.error('Server error trying to send email.');
  //         }
  //       );
  //   }
  // }

  // onClose() {
  //   this.dialogRef.close();
  // }
}
