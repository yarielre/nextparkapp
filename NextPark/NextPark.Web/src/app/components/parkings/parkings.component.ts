import { distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { fromEvent as observableFromEvent, Observable } from 'rxjs';
import {
  Component,
  OnInit,
  ElementRef,
  ViewChild,
  ChangeDetectorRef
} from '@angular/core';

import { MatPaginator, MatSort } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';

import { ParkingDataSource } from '../../_helpers/data-sources';
import { ParkingsService } from 'src/app/services';
import { Parking } from 'src/app/models';

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
    private changeDetectorRefs: ChangeDetectorRef
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
