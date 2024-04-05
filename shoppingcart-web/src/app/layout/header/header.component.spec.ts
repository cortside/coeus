import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { LayoutFacade } from '../layout.facade';

import { HeaderComponent } from './header.component';

describe('HeaderComponent', () => {
    let component: HeaderComponent;
    let fixture: ComponentFixture<HeaderComponent>;

    beforeEach(() => {
        TestBed.configureTestingModule({
            declarations: [HeaderComponent],
            providers: [{ provide: LayoutFacade, useValue: { getCartCount: () => of(3) } }],
        });
        fixture = TestBed.createComponent(HeaderComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
