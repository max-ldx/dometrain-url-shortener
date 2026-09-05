import "./logout-button.css";

function LogoutButton({ onLogout }) {
    return (
        <button onClick={onLogout} className="logout">Logout</button>
    );

}

export default LogoutButton;