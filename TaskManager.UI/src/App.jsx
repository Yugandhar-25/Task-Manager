import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import {AddTaskPage, UpdateTaskPage, LoginPage, RegisterPage, TasksPage} from "./pages";

function App(){
  return(
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/login"/>} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/tasks" element={<TasksPage />} />
        <Route path="/tasks/add" element={<AddTaskPage />} />
        <Route path="/tasks/:id/edit" element={<UpdateTaskPage />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App;