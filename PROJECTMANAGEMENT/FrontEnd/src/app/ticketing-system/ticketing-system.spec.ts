import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketingSystem } from './ticketing-system';

describe('TicketingSystem', () => {
  let component: TicketingSystem;
  let fixture: ComponentFixture<TicketingSystem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TicketingSystem]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TicketingSystem);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
