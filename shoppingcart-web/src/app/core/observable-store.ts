import { BehaviorSubject, ReplaySubject } from "rxjs";

export class ObservableStore<T> {
    private state$: BehaviorSubject<T>;

    constructor(initialValues: T) {
        this.state$ = new BehaviorSubject(initialValues);
    }

    patch(partial: Partial<T>) {
        // TODO
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