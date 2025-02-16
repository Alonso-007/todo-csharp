import { Dispatch } from "react";
import { ActionTypes } from "./common";
import config from "../config"
import { ActionMethod, createPayloadAction, PayloadAction } from "./actionCreators";
import { VersionService } from "../services/versionService";
import { AppVersion } from "../models/appVersion";

const versionService = new VersionService(config.api.baseUrl, '/version');

export interface VersionActions {
    get(): Promise<AppVersion>
}

export const get = (): ActionMethod<AppVersion> => async (dispatch: Dispatch<GetVersionAction>) => {
    
    const version = await versionService.get("");

    dispatch(getVersionAction(version));

    return version;
}

export interface GetVersionAction extends PayloadAction<string, AppVersion> {
    type: ActionTypes.GET_APP_VERSION
}

const getVersionAction = createPayloadAction<GetVersionAction>(ActionTypes.GET_APP_VERSION);