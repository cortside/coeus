import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AuthenticationService } from '@muziehdesign/core';
import { AppConfig } from 'src/environments/app-config';
import { AppComponent } from './app.component';
import { FooterComponent } from './layout/footer/footer.component';

describe('AppComponent', () => {
    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [RouterTestingModule],
            declarations: [AppComponent, FooterComponent],
            providers: [
                { provide: AppConfig, useValue: {} },
                { provide: AuthenticationService, useValue: {} },
            ],
        }).compileComponents();
    });

    it('should create the app', () => {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.componentInstance;
        expect(app).toBeTruthy();
    });
});
