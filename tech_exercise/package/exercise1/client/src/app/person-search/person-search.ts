import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-person-search',
  imports: [CommonModule],
  templateUrl: './person-search.html',
  styleUrl: './person-search.css',
})
export class PersonSearch implements OnInit {

  http = inject(HttpClient)

  ngOnInit(): void {
    this.http.get("api/Person")
      .subscribe((res) =>{
        console.log(res);
      })
    //throw new Error('Method not implemented.');
  }

}
