import { Parking } from './../../models/parking.model';
import { DataSource } from '@angular/cdk/table';
import { merge as observableMerge, BehaviorSubject, Observable } from 'rxjs';
import { MatPaginator, MatSort } from '@angular/material';
import { map, finalize } from 'rxjs/operators';

export class ParkingDataSource extends DataSource<Parking> {
  _filterChange = new BehaviorSubject('');
  get filter(): string {
    return this._filterChange.value;
  }
  set filter(filter: string) {
    this._filterChange.next(filter);
  }

  filteredData: Parking[] = [];
  renderedData: Parking[] = [];
  totalItems: Number;

  constructor(
    private _paginator: MatPaginator,
    private _sort: MatSort,
    private data: Parking[]
  ) {
    super();
    this._filterChange.subscribe(() => (this._paginator.pageIndex = 0));
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<Parking[]> {
    // Listen for any changes in the base data, sorting, filtering, or pagination
    const displayDataChanges = [
      this._sort.sortChange,
      this._filterChange,
      this._paginator.page
    ];

    return observableMerge(...displayDataChanges).pipe(
      map(() => {
        this.filteredData = this.data.slice().filter((item: Parking) => {
          let searchStr = item.address.toLowerCase();
          return searchStr.indexOf(this.filter.toLowerCase()) !== -1;
        });
        const sortedData = this.sortData(this.filteredData.slice());
        const startIndex = this._paginator.pageIndex * this._paginator.pageSize;
        this.renderedData = sortedData.splice(
          startIndex,
          this._paginator.pageSize
        );
        return this.renderedData;
      })
    );
  }

  disconnect() {}

  sortData(data: Parking[]): Parking[] {
    if (!this._sort.active || this._sort.direction === '') {
      return data;
    }

    return data.sort((a, b) => {
      let propertyA: number | string = '';
      let propertyB: number | string = '';

      switch (this._sort.active) {
        case 'id':
          [propertyA, propertyB] = [a.id, b.id];
          break;
        // case 'username':
        //   [propertyA, propertyB] = [a.name, b.name];
        //   break;
        // case 'progress':
        //   [propertyA, propertyB] = [a.progress, b.progress];
        //   break;
        // case 'color':
        //   [propertyA, propertyB] = [a.color, b.color];
        //   break;
      }

      let valueA = isNaN(+propertyA) ? propertyA : +propertyA;
      let valueB = isNaN(+propertyB) ? propertyB : +propertyB;

      return (
        (valueA < valueB ? -1 : 1) * (this._sort.direction === 'asc' ? 1 : -1)
      );
    });
  }
}
