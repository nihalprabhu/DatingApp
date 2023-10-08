import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/users';
import { Router } from '@angular/router';
import {ToastrService} from 'ngx-toastr'

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  
  model: any ={}
  currentUser$: Observable<User| null> = of(null)
  constructor(private accountService: AccountService, private router:Router, private toastr: ToastrService) {

  }
  ngOnInit(): void {
    this.currentUser$= this.accountService.currentUser$;
  }


  login(){
    this.accountService.login(this.model).subscribe({
      next: _ => {
        this.router.navigateByUrl('/members')
      },
      error: err=> {
      }
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');

  }


}
