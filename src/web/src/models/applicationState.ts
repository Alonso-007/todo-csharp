import { Dispatch } from "react";
import { TodoActions } from "../actions/common";
import { TodoItem } from "./todoItem";
import { TodoList } from "./todoList";
import { AppVersion } from "./appVersion";

export interface AppContext {
    state: ApplicationState
    dispatch: Dispatch<TodoActions>
}

export interface ApplicationState {
    lists?: TodoList[]
    selectedList?: TodoList
    selectedItem?: TodoItem
    version?: AppVersion
}

export const getDefaultState = (): ApplicationState => {
    return {
        lists: undefined,
        selectedList: undefined,
        selectedItem: undefined,
        version: undefined
    }
}

