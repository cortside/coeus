import { Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationStart, Router, RouterEvent } from '@angular/router';
import { filter } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class RouteContextData {

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private data = new Map<string, any>();
    constructor(private router: Router) {
        this.router.events
            .pipe(
                takeUntilDestroyed(),
                filter((e) => e instanceof RouterEvent && e instanceof NavigationStart)
            )
            .subscribe(() => this.data.clear());
    }

    set<T>(key: string, value: T) {
        this.data.set(key, value);
    }

    get<T>(key: string): T | undefined {
        return this.data.get(key);
    }
}
