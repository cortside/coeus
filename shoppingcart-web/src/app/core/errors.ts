export class UnexpectedError extends Error{
    constructor(innerError? : Error, message? : string) {
        super(message || 'Unexpected error');
        this.name = 'Unexpected error';
        this.stack = innerError?.stack;
    }
}