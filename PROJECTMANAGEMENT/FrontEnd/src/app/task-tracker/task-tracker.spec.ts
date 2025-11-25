import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskTracker } from './task-tracker';

describe('TaskTracker', () => {
  let component: TaskTracker;
  let fixture: ComponentFixture<TaskTracker>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskTracker]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TaskTracker);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
