import "./AuctionCountdown.css";

export default function AuctionCountdown() {
    return (
        <section className="auc-card" aria-label="Veiling info">
            <header className="auc-card__head">
                <h3 className="auc-card__title">Veiling start in</h3>
            </header>

            <div className="auc-timer">
                <div className="auc-timer__cell">
                    <span className="auc-timer__value">0</span>
                    <span className="auc-timer__unit">d</span>
                </div>
                <div className="auc-timer__cell">
                    <span className="auc-timer__value">12</span>
                    <span className="auc-timer__unit">d</span>
                </div>
                <div className="auc-timer__cell">
                    <span className="auc-timer__value">55</span>
                    <span className="auc-timer__unit">m</span>
                </div>
                <div className="auc-timer__cell">
                    <span className="auc-timer__value">22</span>
                    <span className="auc-timer__unit">s</span>
                </div>
            </div>

            <hr className="auc-divider" />

            <div className="auc-field">
                <div className="auc-label">Startprijs (max + min)</div>
                <div className="auc-price">
                    <span className="auc-price__main">81c</span>
                    <span className="auc-price__sub">/23c</span>
                </div>
            </div>

            <div className="auc-field">
                <div className="auc-label">Aantal Eenheden</div>
                <div className="auc-qty">
                    <span className="auc-qty__main">200</span>
                    <span className="auc-qty__sub">(3 containers)</span>
                </div>
            </div>
        </section>
    );
}
