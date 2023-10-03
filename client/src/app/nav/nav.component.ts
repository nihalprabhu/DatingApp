import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../models/users';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  
  model: any ={}
  currentUser$: Observable<User| null> = of(null)
  constructor(private accountService: AccountService) {

  }
  ngOnInit(): void {
    this.currentUser$= this.accountService.currentUser$;
  }


  login(){
    this.accountService.login(this.model).subscribe({
      next: response => {console.log(response);},
      error: err=> console.log(err)
    })
  }

  logout(){
    this.accountService.logout();

  }


}
