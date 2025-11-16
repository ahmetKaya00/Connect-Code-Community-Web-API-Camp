import type React from "react";
import { useAuth } from "./AuthContext";
import { Navigate } from "react-router-dom";

const PrivateRoute: React.FC<React.PropsWithChildren> = ({children})=>{
    const {isAuthenticated} = useAuth();
    if(!isAuthenticated)return <Navigate to="/login" replace></Navigate>
    return <>{children}</>
};
export default PrivateRoute;