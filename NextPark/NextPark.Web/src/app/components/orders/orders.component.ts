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

import { OrderDataSource } from '../../_helpers/data-sources';
import { OrdersService } from 'src/app/services/orders.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  showNavListCode;
  displayedColumns = [
    'select',
    'id',
    'price',
    'startDate',
    'endDate',
    'orderStatus',
    'paymentStatus',
    'paymentCode',
    'parkingId',
    'userId'
  ];

  dataSource: OrderDataSource;
  selection = new SelectionModel<string>(true, []);

  constructor(
    private ordersService: OrdersService,
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
    this.ordersService.getAll().subscribe(res => {
      this.dataSource = new OrderDataSource(
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
