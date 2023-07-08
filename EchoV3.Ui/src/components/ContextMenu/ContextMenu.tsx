// react
import React, { useEffect, useRef, useState } from 'react';

// redux
import { useAppDispatch, useAppSelector } from '../../redux/hooks';
import {
  selectChatSettings,
  selectListenerMode,
  selectNameColorMode,
  setChatSettings,
  setListenerMode,
  setNameColorMode,
} from '../../redux/reducers/settingsReducer';

// local
import './ContextMenu.scss';
import { GetCanvasFont, GetTextWidth } from '../../utility/canvas';

type ContextType = 'PLAYER';
interface ContextMenuProps {
  contextType?: ContextType;
  xPos: number;
  yPos: number;
  onClose: () => void;
}

interface ContextSubmenuProps {
  config: SubmenuMenuItemConfig;
}

interface ContextMenuItemProps {
  config: MenuItemConfig;
}

type MenuItemType = 'ACTION' | 'CHECKBOX' | 'SEPARATOR' | 'SUBMENU';
interface GeneralMenuItemConfig {
  title?: string;
  type: MenuItemType;
}

interface ActionMenuItemConfig extends GeneralMenuItemConfig {
  title: string;
  type: 'ACTION';
  onClick?: () => void;
}

interface CheckboxMenuItemConfig extends GeneralMenuItemConfig {
  title: string;
  type: 'CHECKBOX';
  checked?: boolean;
  onClick?: () => void;
}

interface SeparatorMenuItemConfig extends GeneralMenuItemConfig {
  type: 'SEPARATOR';
}

interface SubmenuMenuItemConfig extends GeneralMenuItemConfig {
  title: string;
  type: 'SUBMENU';
  submenu: MenuItemConfig[];
}

type MenuItemConfig =
  | ActionMenuItemConfig
  | CheckboxMenuItemConfig
  | SeparatorMenuItemConfig
  | SubmenuMenuItemConfig;

const ContextMenuItem = ({ config }: ContextMenuItemProps) => {
  switch (config.type) {
    case 'CHECKBOX':
      return (
        <div className="context-menu-item" onClick={config.onClick}>
          <p>{config.title}</p>
          <span
            className={`context-menu-checkbox${config.checked ? ' checked' : ''}`}
          />
        </div>
      );
    case 'SEPARATOR':
      return <div className="context-menu-separator" />;
    case 'SUBMENU':
      return <ContextSubmenu config={config} />;
    default:
      break;
  }
  return (
    <div className="context-menu-item">
      <p>{config.title}</p>
    </div>
  );
};

const ContextSubmenu = ({ config }: ContextSubmenuProps) => {
  const rowRef = useRef<HTMLDivElement | null>(null);
  const submenuRef = useRef<HTMLDivElement | null>(null);
  const [showSubmenu, setShowSubmenu] = useState<boolean>(false);
  const [contextWidth, setContextWidth] = useState<number>(0);
  const [contextHeight, setContextHeight] = useState<number>(0);

  // handle close
  useEffect(() => {
    const handleClose = ({ target }: MouseEvent) => {
      if (!rowRef.current?.contains(target as Node)) {
        if (!submenuRef.current?.contains(target as Node)) {
          setShowSubmenu(false);
        }
      }
    };
    if (showSubmenu) {
      window.addEventListener('mousemove', handleClose);
    }
    return () => {
      window.removeEventListener('mousemove', handleClose);
    };
  }, [showSubmenu]);

  // handle submenu positioning
  useEffect(() => {
    let max = 0;
    let height = 8; // padding
    for (let i = 0; i < config.submenu.length; i++) {
      const menuItem = config.submenu[i];
      if (menuItem.type !== 'SEPARATOR') {
        // text width + padding (both sides) + border
        const titleWidth = GetTextWidth(menuItem.title, GetCanvasFont()) + 48 + 2;
        if (titleWidth > max) {
          max = titleWidth;
        }
        height += 20 + 8; // line height + padding
      } else {
        height += 1;
      }
      height += 4; // gap
    }
    setContextWidth(max);
    setContextHeight(height);
  }, [config.submenu]);

  const clientRect = rowRef.current?.getBoundingClientRect();
  const submenuPositionStyle: React.CSSProperties = {
    position: 'fixed',
    left: clientRect?.left,
    top: clientRect?.top,
  };
  if (clientRect) {
    submenuPositionStyle.left = clientRect.left + clientRect.width;
    if (submenuPositionStyle.left + contextWidth >= window.innerWidth) {
      submenuPositionStyle.left = clientRect.left - contextWidth;
      if (submenuPositionStyle.left + contextWidth >= window.innerWidth) {
        submenuPositionStyle.left =
          window.innerWidth - clientRect.width - contextWidth;
      }
    }
    if (clientRect.top + contextHeight >= window.innerHeight) {
      submenuPositionStyle.top = window.innerHeight - contextHeight;
    }
  }
  return (
    <>
      <div
        ref={rowRef}
        className={`context-menu-item${showSubmenu ? ' active' : ''}`}
        onMouseEnter={() => setShowSubmenu(true)}
      >
        <p>{config.title}</p>
        <span className="context-menu-chevron" />
      </div>
      {showSubmenu && (
        <div ref={submenuRef} className="context-menu" style={submenuPositionStyle}>
          {config.submenu.map((item, idx) => (
            <ContextMenuItem key={`contextmenu-${idx}`} config={item} />
          ))}
        </div>
      )}
    </>
  );
};

const ContextMenu = ({ contextType, xPos, yPos, onClose }: ContextMenuProps) => {
  const dispatch = useAppDispatch();
  const chatSettings = useAppSelector(selectChatSettings);
  const listenerMode = useAppSelector(selectListenerMode);
  const nameColorMode = useAppSelector(selectNameColorMode);
  const [contextWidth, setContextWidth] = useState<number>(0);
  const [contextHeight, setContextHeight] = useState<number>(0);
  const menuRef = useRef<HTMLDivElement | null>(null);

  // DEFAULT CONTEXT MENU
  // Listener Mode []
  // -- Separator --
  // Name Color Strategy > Custom, Random, Job-Based
  // Chat Channel Settings > Say, Emote, Yell, Shout, Tell
  // -- Separator --
  // Clear Ignore List (if playercount > 0)
  // Clear Message History

  // PLAYER CONTEXT MENU
  // Player Name (label)
  // -- Separator --
  // Set Name Color
  // Add to Ignore List

  const menuItems: MenuItemConfig[] = [
    {
      title: 'Listener Mode',
      type: 'CHECKBOX',
      checked: listenerMode,
      onClick: () => dispatch(setListenerMode(!listenerMode)),
    },
    {
      type: 'SEPARATOR',
    },
    {
      title: 'Name Color Strategy',
      type: 'SUBMENU',
      submenu: [
        {
          title: 'Custom',
          type: 'CHECKBOX',
          checked: nameColorMode === 'CUSTOM',
          onClick: () => dispatch(setNameColorMode('CUSTOM')),
        },
        {
          title: 'Random',
          type: 'CHECKBOX',
          checked: nameColorMode === 'RANDOM',
          onClick: () => dispatch(setNameColorMode('RANDOM')),
        },
        {
          title: 'Job-Based',
          type: 'CHECKBOX',
          checked: nameColorMode === 'JOB',
          onClick: () => dispatch(setNameColorMode('JOB')),
        },
      ],
    },
    {
      title: 'Chat Channel Settings',
      type: 'SUBMENU',
      submenu: [
        {
          title: 'Say',
          type: 'CHECKBOX',
          checked: chatSettings.Say,
          onClick: () => dispatch(setChatSettings({ Say: !chatSettings.Say })),
        },
        {
          title: 'Emote',
          type: 'CHECKBOX',
          checked: chatSettings.Emote,
          onClick: () => dispatch(setChatSettings({ Emote: !chatSettings.Emote })),
        },
        {
          type: 'SEPARATOR',
        },
        {
          title: 'Shout',
          type: 'CHECKBOX',
          checked: chatSettings.Shout,
          onClick: () => dispatch(setChatSettings({ Shout: !chatSettings.Shout })),
        },
        {
          title: 'Yell',
          type: 'CHECKBOX',
          checked: chatSettings.Yell,
          onClick: () => dispatch(setChatSettings({ Yell: !chatSettings.Yell })),
        },
      ],
    },
    {
      type: 'SEPARATOR',
    },
    {
      title: 'Clear Message History',
      type: 'ACTION',
    },
  ];

  // menu items
  useEffect(() => {
    let max = 0;
    let height = 8; // padding
    for (let i = 0; i < menuItems.length; i++) {
      const menuItem = menuItems[i];
      if (menuItem.type !== 'SEPARATOR') {
        // text width + padding (both sides) + border
        const titleWidth = GetTextWidth(menuItem.title, GetCanvasFont()) + 48 + 2;
        if (titleWidth > max) {
          max = titleWidth;
        }
        height += 20 + 8; // line height + padding
      } else {
        height += 1;
      }
      height += 4; // gap
    }
    setContextWidth(max);
    setContextHeight(height);
  }, [menuItems]);

  // handle close
  useEffect(() => {
    const handleClick = ({ target }: MouseEvent) => {
      if (!menuRef.current || !menuRef.current.contains(target as Node)) {
        onClose();
      }
    };
    window.addEventListener('blur', onClose);
    window.addEventListener('mousedown', handleClick);
    return () => {
      window.removeEventListener('blur', onClose);
      window.removeEventListener('mousedown', handleClick);
    };
  }, []);

  const positionStyle: React.CSSProperties = {
    position: 'absolute',
    left: xPos,
    top: yPos,
  };
  if (xPos + contextWidth >= window.innerWidth) {
    positionStyle.left = window.innerWidth - contextWidth;
  }
  if (yPos + contextHeight >= window.innerHeight) {
    positionStyle.top = window.innerHeight - contextHeight;
  }
  return (
    <div ref={menuRef} className="context-menu" style={positionStyle}>
      {menuItems.map((item, idx) => (
        <ContextMenuItem key={`contextmenu-${idx}`} config={item} />
      ))}
    </div>
  );
};
export default ContextMenu;
