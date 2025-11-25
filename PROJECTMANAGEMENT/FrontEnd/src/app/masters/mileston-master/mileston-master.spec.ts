import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MilestonMaster } from './mileston-master';

describe('MilestonMaster', () => {
  let component: MilestonMaster;
  let fixture: ComponentFixture<MilestonMaster>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MilestonMaster]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MilestonMaster);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
