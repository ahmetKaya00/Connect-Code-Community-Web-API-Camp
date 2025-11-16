import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5286",
});

api.interceptors.request.use((config)=>{
    const token = localStorage.getItem("token");
    if(token){
        config.headers = config.headers ?? {};
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

api.interceptors.response.use(
    (r) => r,
    (err) => {
        if(err.response?.status === 401){
            localStorage.removeItem("token");
            localStorage.removeItem("user");
            if(window.location.pathname !== "/login"){
                window.location.href = "/login";
            }
        }
        return Promise.reject(err);
    }
)

export default api;