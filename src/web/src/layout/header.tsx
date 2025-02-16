import { FontIcon, getTheme, IconButton, IIconProps, IStackStyles, mergeStyles, Persona, PersonaSize, Stack, Text } from '@fluentui/react';
import { FC, ReactElement, useContext, useEffect, useMemo } from 'react';
import * as versionActions from '../actions/versionActions';
import { VersionActions } from '../actions/versionActions';
import { bindActionCreators } from '../actions/actionCreators';
import { AppContext } from '../models/applicationState';
import { TodoContext } from '../components/todoContext';

const theme = getTheme();

const logoStyles: IStackStyles = {
    root: {
        width: '300px',
        background: theme.palette.themePrimary,
        alignItems: 'center',
        padding: '0 20px'
    }
}

const logoIconClass = mergeStyles({
    fontSize: 20,
    paddingRight: 10
});

const toolStackClass: IStackStyles = {
    root: {
        alignItems: 'center',
        height: 48,
        paddingRight: 10
    }
}

const iconProps: IIconProps = {
    styles: {
        root: {
            fontSize: 16,
            color: theme.palette.white
        }
    }
}

const Header: FC = (): ReactElement => {
    const appContext = useContext<AppContext>(TodoContext)
    const actions = useMemo(() => ({
        version: bindActionCreators(versionActions, appContext.dispatch) as unknown as VersionActions,
    }), [appContext.dispatch]);    
    
    // Display current version on initial load
    useEffect(() => {
        if (!appContext.state.version?.value) {
            const loadVersion = async () => {
                appContext.state.version = await actions.version.get();
            }
            loadVersion();
        }
    });

    return (
        <Stack horizontal>
            <Stack horizontal styles={logoStyles}>
                <FontIcon aria-label="Check" iconName="SkypeCircleCheck" className={logoIconClass} />
                <Text variant="xLarge">ToDo - {appContext.state.version?.value}</Text>
            </Stack>
            <Stack.Item grow={1}>
                <div></div>
            </Stack.Item>
            <Stack.Item>
                <Stack horizontal styles={toolStackClass} grow={1}>
                    <IconButton aria-label="Add" iconProps={{ iconName: "Settings", ...iconProps }} />
                    <IconButton aria-label="Add" iconProps={{ iconName: "Help", ...iconProps }} />
                    <Persona size={PersonaSize.size24} text="Sample User" />
                    {/* <Toggle label="Dark Mode" inlineLabel styles={{ root: { marginBottom: 0 } }} onChange={changeTheme} /> */}
                </Stack>
            </Stack.Item>
        </Stack>
    );
}

export default Header;