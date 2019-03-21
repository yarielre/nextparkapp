import { ParkingDeleteConfirmDialogComponent } from './../_shared/delete-confirm-dialog/parking-delete-confirm-dialog.component.1';
import { distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { fromEvent as observableFromEvent, Observable } from 'rxjs';
import {
  Component,
  OnInit,
  ElementRef,
  ViewChild,
  ChangeDetectorRef
} from '@angular/core';

import { MatPaginator, MatSort, MatDialogConfig, MatDialog } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';

import { ParkingDataSource } from '../../_helpers/data-sources';
import { NotificationService } from 'src/app/services/notification.service';
import { ParkingsService } from 'src/app/services/parkings.service';
import { ParkingFormComponent } from '../_forms/parking-form/parking-form.component';

@Component({
  selector: 'app-parkings',
  templateUrl: './parkings.component.html',
  styleUrls: ['./parkings.component.scss']
})

export class ParkingsComponent implements OnInit {
  showNavListCode;
  displayedColumns = [
    'select',
    'id',
    'address',
    'cap',
    'city',
    'state',
    'carPlate',
    'latitude',
    'longitude',
    'priceMin',
    'priceMax',
    'status',
  ];

  dataSource: ParkingDataSource;
  selection = new SelectionModel<string>(true, []);

  constructor(
    private parkingService: ParkingsService,
    private changeDetectorRefs: ChangeDetectorRef,
    private dialog: MatDialog,
    private notifService: NotificationService
  ) {}

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('filter') filter: ElementRef;

  ngOnInit() {
    this.refresh();

    observableFromEvent(this.filter.nativeElement, 'keyup')
      .pipe(distinctUntilChanged())
      .subscribe(() => {
        if (!this.dataSource) {
          return;
        }
        this.dataSource.filter = this.filter.nativeElement.value;
      });
  }

  onDelete(selected: string[]) {
    this.openDeleteDialog(selected);
  }

  openDeleteDialog(selected: string[]) {
    const parkings = this.dataSource.renderedData.filter(x =>
      selected.includes(x.id.toString())
    );
    const dialogConfig: MatDialogConfig = new MatDialogConfig();
    const message =
    parkings.length > 1
        ? `Are you sure you want to delete all parkings`
        : `Are you sure you want to delete this parking <em>${
          parkings[0].address
          }.</em>`;
    dialogConfig.autoFocus = true;
    dialogConfig.width = "40%";
    dialogConfig.data = {
      title: "Delete Parking",
      text: message,
      payload: parkings.map(x => x.id)
    };

    this.dialog
      .open(ParkingDeleteConfirmDialogComponent, dialogConfig)
      .afterClosed()
      .subscribe(deleteDialogdata => {
        if (deleteDialogdata !== undefined && deleteDialogdata.isOnDelete) {
          this.refresh();
          this.notifService.success("Deleted successfully.");
        }
      });
  }

  onEdit(selected: string[]) {
    const parking = this.dataSource.renderedData.filter(x =>
      selected.includes(x.id.toString())
    );

    this.parkingService.fillForm(parking[0]);
    this.openEditDialog();
  }

  openEditDialog() {
    const dialogConfig: MatDialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    dialogConfig.width = "50%";
    this.dialog
      .open(ParkingFormComponent, dialogConfig)
      .afterClosed()
      .subscribe(parkingEditedResult => {
        if (parkingEditedResult !== undefined && parkingEditedResult.isUpdated) {
          if (parkingEditedResult.payload !== undefined) {
            this.refresh();
            this.notifService.success("Parking updated.");
          } else {
            this.notifService.error("Server error");
          }
        }
      });
  }

  refresh() {
    this.parkingService.getAll().subscribe(res => {
      this.dataSource = new ParkingDataSource(
        this.paginator,
        this.sort,
        res
      );
      this.changeDetectorRefs.detectChanges();
    });
  }

  isAllSelected(): boolean {
    if (!this.dataSource) {
      return false;
    }
    if (this.selection.isEmpty()) {
      return false;
    }
    return (
      this.selection.selected.length === this.dataSource.renderedData.length
    );
  }

  masterToggle() {
    if (!this.dataSource) {
      return;
    }

    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.dataSource.renderedData.forEach(data =>
        this.selection.select(data.id.toString())
      );
    }
    console.log('master: ', this.selection.selected);
  }
}
