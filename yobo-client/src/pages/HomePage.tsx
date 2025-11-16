import type React from "react";
import { useAuth } from "../auth/AuthContext";

const HomePage: React.FC = () => {
    const { user, logout} = useAuth();

    return(
        <div style={{maxWidth:720,margin:"40px auto"}}>
            <h1>Home</h1>
            <p>Hoş Geldin <b>{user?.fullName ?? user?.email}</b>!</p>
            <button onClick={logout}>Çıkış Yap</button>
        </div>
    );
}
export default HomePage;