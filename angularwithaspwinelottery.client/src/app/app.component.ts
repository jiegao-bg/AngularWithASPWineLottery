import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { DataService } from './data.service'; // Import data service

interface LotteryUser {
  date: string;
  summary: string;
  name: string;
  id: string;
  ticket: number;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {

  public forecasts: LotteryUser[] = [];
  userForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private dataService: DataService) {
    this.userForm = this.fb.group({
      name: ['',
        Validators.required,
        Validators.minLength(2), Validators.maxLength(20)
      ],
      id: ['',
        [
          Validators.required,
          //Validators.pattern(/^[a-zA-Z]{2}$/)
        ],
      ],
      ticket: ['',
        [
          Validators.required
        ],
      ],
      comment: ['',
        [
          
        ],
      ]
    });

  }

  ngOnInit() {
    this.getForecasts();
  }

  clear() {
    this.userForm.reset();
  }

  getForecasts() {
    this.http.get<LotteryUser[]>('https://localhost:7255/winelottery').subscribe(
      (result) => {
        this.forecasts = result;
        if (result.length == 0) {
          this.ticketNumber = 100;
        } else {
          for (let item of result) {
            this.sum = this.sum + item.ticket;
          }

          this.ticketNumber = this.ticketNumber - this.sum;
        }
      },
      (error) => {
        console.error(error);
      }
    );
  }

 getRemainingTickets() {

    var ticket = this.userForm.controls['ticket'].value;

    /** GET: value from the file or database */
    this.dataService.getNumberByTicket(ticket).subscribe({
      next: (responseData: number) => {
        console.log(responseData); // Handle the response data
        this.ticketNumber = responseData;
      },
      error: (error) => {
        console.error('There was an error!', error);
      }
    });

    //set all value to null
    this.userForm.reset();
  }

  onSubmit() {
    var register = <LotteryUser>{};
  
    register.name = this.userForm.controls['name'].value;
    register.id = this.userForm.controls['id'].value;
    register.ticket = this.userForm.controls['ticket'].value;
    register.summary = this.userForm.controls['comment'].value;

    if (register.name !== "" && register.id !== "" && register.ticket !== null) {
      if (register.ticket <= this.ticketNumber) {

        /** POST: add a new to the file or database */
        this.dataService.postData(register).subscribe({
          next: (responseData) => {
            console.log(responseData); // Handle the response data
            this.getForecasts();
            this.getRemainingTickets();
          },
          error: (error) => {
            console.error('There was an error!', error);
          }
        });
      }else{
        alert('Ticket number is more than the remaining tickets');
      }
    }else{
      alert('Please enter name, ticket id or ticket number');
    }
  }

  title = 'Remaining total tickets: ';
  ticketNumber = 100;
  sum = 0;
}
