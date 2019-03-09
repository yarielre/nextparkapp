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

import { EventDataSource } from '../../_helpers/data-sources';
import { EventsService } from 'src/app/services';
import { Event } from 'src/app/models';

@Component({
  selector: 'app-events',
  templateUrl: './events.component.html',
  styleUrls: ['./events.component.scss']
})
export class EventsComponent implements OnInit {
  showNavListCode;
  displayedColumns = [
    'select',
    'id',
    'startDate',
    'endDate',
    'repetitionEndDate',
    'repetitionType',
    'parkingId'
  ];

  dataSource: EventDataSource;
  selection = new SelectionModel<string>(true, []);

  constructor(
    private eventsService: EventsService,
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
    this.eventsService.getAll().subscribe(res => {
      this.dataSource = new EventDataSource(this.paginator, this.sort, res);
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
