import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClientMaster } from './client-master';

describe('ClientMaster', () => {
  let component: ClientMaster;
  let fixture: ComponentFixture<ClientMaster>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientMaster]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientMaster);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
