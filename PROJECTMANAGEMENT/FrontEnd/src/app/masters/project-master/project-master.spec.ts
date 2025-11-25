import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectMaster } from './project-master';

describe('ProjectMaster', () => {
  let component: ProjectMaster;
  let fixture: ComponentFixture<ProjectMaster>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectMaster]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectMaster);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
