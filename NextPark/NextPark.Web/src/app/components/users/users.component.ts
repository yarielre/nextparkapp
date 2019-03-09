import { distinctUntilChanged, debounceTime } from "rxjs/operators";
import { fromEvent as observableFromEvent, Observable } from "rxjs";
import {
  Component,
  OnInit,
  ElementRef,
  ViewChild,
  ChangeDetectorRef
} from "@angular/core";

import { MatPaginator, MatSort } from "@angular/material";
import { SelectionModel } from "@angular/cdk/collections";

import { UserDataSource } from "../../_helpers/data-sources";
import { UsersService } from "src/app/services";
import { User } from "src/app/models";

@Component({
  selector: "app-users",
  templateUrl: "./users.component.html",
  styleUrls: ["./users.component.scss"]
})
export class UsersComponent implements OnInit {
  showNavListCode;
  displayedColumns = [
    "select",
    "id",
    "name",
    "lastname",
    "username",
    "email",
    "phone",
    "address",
    "cap",
    "city",
    "state",
    "carPlate",
    "balance",
    "profit"
  ];

  dataSource: UserDataSource;
  dataAPI: User[];

  selection = new SelectionModel<string>(true, []);

  constructor(
    private userService: UsersService,
    private changeDetectorRefs: ChangeDetectorRef
  ) {}

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild("filter") filter: ElementRef;

  ngOnInit() {
    this.refresh();

    observableFromEvent(this.filter.nativeElement, "keyup")
      .pipe(distinctUntilChanged())
      .subscribe(() => {
        if (!this.dataSource) {
          return;
        }
        console.log("this.filter.nativeElement.value: ",this.filter.nativeElement.value)
        this.dataSource.filter = this.filter.nativeElement.value;
      });
  }

  refresh() {
    this.userService.getAll().subscribe(res => {
      this.dataSource = new UserDataSource(
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
    console.log("master: ", this.selection.selected);
  }
}
