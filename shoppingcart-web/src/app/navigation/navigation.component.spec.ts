import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthenticationService } from '@muziehdesign/core';

import { NavigationComponent } from './navigation.component';

describe('NavigationComponent', () => {
    let component: NavigationComponent;
    let fixture: ComponentFixture<NavigationComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [NavigationComponent],
            providers: [{ provide: AuthenticationService, useValue: {} }],
        }).compileComponents();

        fixture = TestBed.createComponent(NavigationComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
