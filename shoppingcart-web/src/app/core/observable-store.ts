import { BehaviorSubject } from "rxjs";

export class ObservableStore<T> {
    private state$: BehaviorSubject<T>;

    constructor(initialValues: T) {
        this.state$ = new BehaviorSubject(initialValues);
    }

    patch(partial: Partial<T>) {
        const snapshot = this.state$.getValue();
        this.state$.next({...snapshot, ...partial});
    }

    set(newValue: T) {
        this.state$.next(newValue);
    }

    getSnapshot() {
        return this.state$.getValue();
    }

    stateChanges() {
        return this.state$.asObservable();
    }
}