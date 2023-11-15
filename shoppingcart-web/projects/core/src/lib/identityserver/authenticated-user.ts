export class AuthenticatedUser {
    constructor(private profile: Map<string, any>) {}

    get username() {
        return this.profile.get('upn');
    }

    get name() {
        return this.profile.get('name');
    }

    get subjectId() {
        return this.profile.get('sub');
    }

    get provider() {
        return this.profile.get('idp');
    }
}
