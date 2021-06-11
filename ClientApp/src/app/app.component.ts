import { Component, OnInit } from '@angular/core';
import { ISearchInput } from 'data/interfaces/searchInput';
import { EpubService } from 'src/services/epubService.service';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss']
})
export class AppComponent implements OnInit {
  searchInput: ISearchInput;
  sourcePath: string;
  searchParams: string;

  constructor(private epubService: EpubService) {}

  ngOnInit() {
    this.searchInput = <ISearchInput>{};
  }

  SubmitInformationToBackend() {
    if (this.sourcePath === '' || this.searchParams === '' ||
       !this.searchParams || !this.sourcePath) {
      alert('Some of your inputs are empty. Try again, friend!');
      return;
    }

    this.searchInput.searchParams = this.searchParams;
    this.searchInput.sourcePath = this.sourcePath;

    this.epubService.sendInformation(this.searchInput).subscribe(
      o => alert(o),
      err => console.log(err));
  }
}

