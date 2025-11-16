import React, { createContext, useContext, useEffect, useState } from "react";

type User = {
    email: string;
    fullName?: string | null;
}

type AuthContextType = {
    user: User | null;
    token: string | null;
    login: (token:string,user:User) => void;
    logout: () => void;
    isAuthenticated:  boolean;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<React.PropsWithChildren> = ({children}) => {
    const [token, setToken] = useState<string | null>(null);
    const [user, setUser] = useState<User | null>(null);

    useEffect(()=>{
        const t = localStorage.getItem("token");
        const u = localStorage.getItem("user");
        if(t)setToken(t);
        if(u)setUser(JSON.parse(u));
    },[]);

    const login = (t: string, u: User) => {
        setToken(t);
        setUser(u);
        localStorage.setItem("token",t);
        localStorage.setItem("user",JSON.stringify(u));
    };
    const logout = () =>{
        setToken(null);
        setUser(null);
        localStorage.removeItem("token");
        localStorage.removeItem("user");
    }

    return(
        <AuthContext.Provider value={{user,token,login,logout,isAuthenticated: !!token}}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const ctx = useContext(AuthContext);
    if(!ctx) throw new Error("useAuth must be uset...")
    return ctx;
};