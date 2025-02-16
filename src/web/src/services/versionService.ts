import { AppVersion } from '../models/appVersion';
import { RestService } from './restService';

export class VersionService extends RestService<AppVersion> {
    public constructor(baseUrl: string, baseRoute: string) {
        super(baseUrl, baseRoute);
    }
}
