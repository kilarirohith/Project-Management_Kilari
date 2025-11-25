import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VendorMaster } from './vendor-master';

describe('VendorMaster', () => {
  let component: VendorMaster;
  let fixture: ComponentFixture<VendorMaster>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VendorMaster]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VendorMaster);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
