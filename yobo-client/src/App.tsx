import { Navigate, Route, Routes } from "react-router-dom";
import PrivateRoute from "./auth/PrivateRoute";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";

const App: React.FC = () => {
  return(
    <Routes>
      <Route path="/" element={<PrivateRoute><HomePage></HomePage></PrivateRoute>}></Route>
      <Route path="/login" element={<LoginPage></LoginPage>}></Route>
      <Route path="/register" element={<RegisterPage></RegisterPage>}></Route>
      <Route path="*" element={<Navigate to="/" replace></Navigate>}></Route>
    </Routes>
  );
}
export default App;