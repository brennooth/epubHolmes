import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ISearchInput } from 'data/interfaces/searchInput';

@Injectable()
export class EpubService {
    constructor(private http: HttpClient) {}

    sendInformation(searchInput: ISearchInput) {
        const url = 'api/Epub';
        return this.http
            .post(url, searchInput);
    }
}
