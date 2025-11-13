import "./AuctionCountdown.css";
import { useEffect, useState } from "react";

function getNextNov15(): Date {
    const now = new Date();
    const year =
        now.getMonth() > 10 || (now.getMonth() === 10 && now.getDate() > 15)
            ? now.getFullYear() + 1
            : now.getFullYear();
    return new Date(year, 10, 15, 0, 0, 0, 0);
}

function getTimePartsUntil(target: Date) {
    let ms = target.getTime() - Date.now();
    if (ms < 0) ms = 0;
    const totalSec = Math.floor(ms / 1000);
    const days = Math.floor(totalSec / 86400);
    const hours = Math.floor((totalSec % 86400) / 3600);
    const minutes = Math.floor((totalSec % 3600) / 60);
    const seconds = totalSec % 60;
    return { days, hours, minutes, seconds };
}export default function AuctionCountdown({
    price,
    quantity,
    containers
}: {
    price: string;
    quantity: string;
    containers: string;
}) {
    const [t, setT] = useState(() => getTimePartsUntil(getNextNov15()));

    useEffect(() => {
        const id = setInterval(() => {
            setT(getTimePartsUntil(getNextNov15()));
        }, 1000);
        return () => clearInterval(id);
    }, []);

    return (
        <section className="auc-card" aria-label="Veiling info">
            <header className="auc-card__head">
                <h3 className="auc-card__title">Veiling start in</h3>
            </header>

            <div className="auc-timer">
                <div className="auc-timer__cell">
                    <span className="auc-timer__value">{t.days}</span>
                    <span className="auc-timer__unit">d</span>
                </div>
                <div className="auc-timer__cell">
                    <span className="auc-timer__value">{t.hours}</span>
                    <span className="auc-timer__unit">h</span>
                </div>
                <div className="auc-timer__cell">
                    <span className="auc-timer__value">{t.minutes}</span>
                    <span className="auc-timer__unit">m</span>
                </div>
                <div className="auc-timer__cell">
                    <span className="auc-timer__value">{t.seconds}</span>
                    <span className="auc-timer__unit">s</span>
                </div>
            </div>

            <hr className="auc-divider" />

            <div className="auc-field">
                <div className="auc-label">Startprijs</div>
                <div className="auc-price">
                    <span className="auc-price__main">{price}</span>
                </div>
            </div>

            <div className="auc-field">
                <div className="auc-label">Aantal Eenheden</div>
                <div className="auc-qty">
                    <span className="auc-qty__main">{quantity}</span>
                    <span className="auc-qty__sub">{containers}</span>
                </div>
            </div>
        </section>
    );
}
