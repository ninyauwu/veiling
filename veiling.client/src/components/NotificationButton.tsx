// src/components/NotificationButton.tsx
import { useState, useRef, useEffect } from "react";
import "./NotificationButton.css";

interface Notification {
  id: number;
  title: string;
  message: string;
  time: string;
  isRead: boolean;
}

interface NotificationButtonProps {
  notificationCount?: number;
}

function NotificationButton({
  notificationCount = 0,
}: NotificationButtonProps) {
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Dummy notifications
  const notifications: Notification[] = [
    {
      id: 1,
      title: "Verkocht!",
      message: 'Er is geboden op "Zwarte Rozen"',
      time: "2 min geleden",
      isRead: false,
    },
    {
      id: 2,
      title: "Veiling eindigt binnenkort",
      message: 'Je veiling "Bismarck Palm Zaden" eindigt over 1 uur',
      time: "30 min geleden",
      isRead: false,
    },
    {
      id: 3,
      title: "Verkocht!",
      message: 'Je item "Cacao Bonen, 20 Kilo" is verkocht',
      time: "1 uur geleden",
      isRead: true,
    },
    {
      id: 3,
      title: "Reminder",
      message: "Je volgt een veiling in Delft, deze start over 15 minuten",
      time: "1 uur geleden",
      isRead: true,
    },
  ];

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (
        dropdownRef.current &&
        !dropdownRef.current.contains(event.target as Node)
      ) {
        setIsOpen(false);
      }
    };

    if (isOpen) {
      document.addEventListener("mousedown", handleClickOutside);
    }

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [isOpen]);

  return (
    <div className="notification-container" ref={dropdownRef}>
      <button
        className="notification-button"
        onClick={() => setIsOpen(!isOpen)}
      >
        <svg
          className="bell-icon"
          viewBox="0 0 24 24"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M18 8C18 6.4087 17.3679 4.88258 16.2426 3.75736C15.1174 2.63214 13.5913 2 12 2C10.4087 2 8.88258 2.63214 7.75736 3.75736C6.63214 4.88258 6 6.4087 6 8C6 15 3 17 3 17H21C21 17 18 15 18 8Z"
            stroke="#7A1F3D"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
          <path
            d="M13.73 21C13.5542 21.3031 13.3019 21.5547 12.9982 21.7295C12.6946 21.9044 12.3504 21.9965 12 21.9965C11.6496 21.9965 11.3054 21.9044 11.0018 21.7295C10.6982 21.5547 10.4458 21.3031 10.27 21"
            stroke="#7A1F3D"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>

        {notificationCount > 0 && (
          <span className="notification-badge">{notificationCount}</span>
        )}
      </button>

      {isOpen && (
        <div className="notification-dropdown">
          <div className="notification-header">
            <h3>Notificaties</h3>
            <button className="clear-btn">Wis alles</button>
          </div>

          <div className="notification-list">
            {notifications.map((notification) => (
              <div
                key={notification.id}
                className={`notification-item ${notification.isRead ? "read" : "unread"}`}
              >
                <div className="notification-content">
                  <h4>{notification.title}</h4>
                  <p>{notification.message}</p>
                  <span className="notification-time">{notification.time}</span>
                </div>
                {!notification.isRead && <span className="unread-dot"></span>}
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

export default NotificationButton;

