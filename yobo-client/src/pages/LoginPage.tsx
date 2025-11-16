import type React from "react";
import { useAuth } from "../auth/AuthContext";
import { Link, useNavigate } from "react-router-dom";
import { useState } from "react";
import api from "../lib/api";

const LoginPage: React.FC = () => {
    const {login} = useAuth();

    const navigate = useNavigate();

    const [email,setEmail] = useState("");
    const [password,setPassword] = useState("");
    const [loading,setLoading] = useState(false);
    const [error,setError] = useState<string | null>(null);


const onSubmit = async (e: React.FormEvent)=>{
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
        const res = await api.post("/api/Auth/login", {email , password});
        const {token, email:outEmail, fullName} = res.data;
        login(token, {email:outEmail,fullName});
        navigate("/");
    } catch (err: any) {
        setError(err.response?.data?.message || "Giriş Başarısız");
    }finally{
        setLoading(false);
    }
};

return(
    <div style={{maxWidth:380, margin: "60px auto"}}>
        <h1>Giriş Yap</h1>
        <form onSubmit={onSubmit}>
            <label style={{display: "block", marginTop: 12}}>E-Posta
            <input type="email" value={email} onChange={e=>setEmail(e.target.value)} required style={{width: "100%", padding:10,marginTop:6}}></input>
            </label>
            <label style={{display: "block", marginTop: 12}}>Password
            <input type="password" value={password} onChange={e=>setPassword(e.target.value)} required style={{width: "100%", padding:10,marginTop:6}}></input>
            </label>
            {error && <div style={{color: "red",marginTop:10}}>{error}</div>}
            <button type="submit" disabled={loading}style={{width: "100%", padding:12,marginTop:16,border:0,borderRadius:8,cursor: "pointer"}}>{loading ? "Gönderiliyor...":"Giriş Yap"}</button>
        </form>
        <p>Hesabınız Yok mu ? <Link to="/register">Kayıt Ol</Link></p>
    </div>
);
};
export default LoginPage;